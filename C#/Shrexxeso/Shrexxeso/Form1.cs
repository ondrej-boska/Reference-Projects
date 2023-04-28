
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shrexxeso
{
    public partial class Form1 : Form
    {
        readonly string[] images = { "image1", "image2", "image3", "image4", "image5", "image6", "image7",
           "image8", "image9", "image10", "image11", "image12", "image13", "image14", "image15",
           "image1", "image2", "image3", "image4", "image5", "image6", "image7", "image8",
           "image9", "image10", "image11", "image12", "image13", "image14", "image15" };

        short numSelected = 0;
        short currPlayer = 0;
        const int waitingTime = 500; // edit this value to make the game go faster/slower

        bool waiting = false;

        readonly PictureBox[] selected = new PictureBox[2];
        readonly PictureBox[] pictureBoxes = new PictureBox[30];
        readonly Player[] players = new Player[2];
        bool[] known = new bool[30];

        readonly ArrayList visibleBoxes = new ArrayList();
        readonly ArrayList visited = new ArrayList();
        readonly Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
            ShowPopupChooseEnemy();
            InitPexxeso();
        }

        private void InitPexxeso()
        {
            SetImages();
            players[0] = new Player("human", "You: ", Label_PointsP1, "You Win!");

            foreach (PictureBox pictureBox in tableLayoutPanel1.Controls)
            {
                pictureBox.Click += new System.EventHandler(PictureBox_Click);
                visibleBoxes.Add(pictureBox);
            }
        }

        private void Randomize()
        {
            for (int i = 0; i < images.Length; i++)
            {
                int next = rnd.Next(0, images.Length - 1);
                string curr = images[i];
                images[i] = images[next];
                images[next] = curr;
            }

        }

        private void SetImages()
        {
            Randomize();

            int i = 0;
            foreach (PictureBox pictureBox in tableLayoutPanel1.Controls)
            {
                TurnCard(pictureBox, "back_cover");
                pictureBox.Tag = images[i];
                pictureBoxes[i] = pictureBox;
                i++;
            }
        }

        private static void TurnCard(PictureBox pictureBox, string image)
        {
            pictureBox.Image = (Image)Properties.Resources.ResourceManager.GetObject(image);
        }

        public void ResetGame_Click(object sender, EventArgs e)
        {
            if (!waiting) {

                ShowPopupChooseEnemy();
                SetImages();

                visibleBoxes.Clear();
                visited.Clear();
                known = new bool[30];

                foreach (PictureBox pictureBox in tableLayoutPanel1.Controls)
                {
                    pictureBox.Visible = true;
                    visibleBoxes.Add(pictureBox);
                }

                for (int i = 0; i < 2; i++)
                    players[i].ChangePoints(0);
            }
        }

        private async void PictureBox_Click(object sender, EventArgs e)
        {
            if(! waiting)
            {
                PictureBox curr = (PictureBox)sender;

                if (numSelected == 1 && curr == selected[0]) return; // Player has selected the same card twice
                
                selected[numSelected] = curr;
                TurnCard(curr, (string)curr.Tag);
                known[FindPictureBoxIndex(curr)] = true;
                numSelected++;
                
                if (numSelected == 2)
                {
                    waiting = true;
                    await Task.Delay(2*waitingTime);
                    waiting = false;

                    EvaluateDouble();

                    numSelected = 0;
                    bool ended = CheckIfEnded();
                    currPlayer = (short)((currPlayer + 1) % 2);

                    if (! ended)
                    {
                        if (players[1].type == "Donkey") MakeDonkeysMove();
                        else MakeShreksMove();
                    } 
                }
            }       
        }

        private void EvaluateDouble()
        {
            if (selected[0].Tag == selected[1].Tag)
            {
                players[currPlayer].ChangePoints(++players[currPlayer].points);
                for (int i = 0; i < 2; i++)
                {
                    selected[i].Visible = false;
                    visibleBoxes.Remove(selected[i]);
                }   
            }
            else
            {
                for (int i = 0; i < 2; i++)
                    TurnCard(selected[i], "back_cover");
            }
        }

        private async void MakeDonkeysMove()
        {
            waiting = true;
            await Task.Delay(waitingTime / 2);
            waiting = false;

            for (int i = 0; i < 2; i++)
            {
                selected[i] = (PictureBox)visibleBoxes[rnd.Next(visibleBoxes.Count)];
                TurnCard(selected[i], (string)selected[i].Tag);
                known[FindPictureBoxIndex(selected[i])] = true;
                selected[i].Refresh();

                visibleBoxes.Remove(selected[i]);

                waiting = true;
                await Task.Delay((i+1) * waitingTime);
                waiting = false;
            }

            if (selected[0].Tag == selected[1].Tag)
            {
                players[currPlayer].ChangePoints(++players[currPlayer].points);
                for (int i = 0; i < 2; i++)
                    selected[i].Visible = false;
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    TurnCard(selected[i], "back_cover");
                    visibleBoxes.Add(selected[i]);
                }  
            }

            CheckIfEnded();
            currPlayer = (short)((currPlayer + 1) % 2);
        }

        private async void MakeShreksMove()
        {
            bool found = false;
            for (int i = 0; i < pictureBoxes.Length; i++)
            {
                for (int j = i; j < pictureBoxes.Length; j++)
                {
                    if((string)pictureBoxes[i].Tag == (string)pictureBoxes[j].Tag && visibleBoxes.Contains(pictureBoxes[i]) && known[i] == true && known[j] == true && i != j)
                    {
                        selected[0] = pictureBoxes[i];
                        selected[1] = pictureBoxes[j];
                        found = true;
                        i = pictureBoxes.Length;
                        j = pictureBoxes.Length;
                    } 
                }
            }

            if (!found) MakeDonkeysMove();
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    TurnCard(selected[i], (string)selected[i].Tag);
                    selected[i].Refresh();

                    visibleBoxes.Remove(selected[i]);      

                    waiting = true;
                    await Task.Delay((i + 1) * 500);
                    waiting = false;
                }

                players[currPlayer].ChangePoints(++players[currPlayer].points);
                for (int i = 0; i < 2; i++)
                    selected[i].Visible = false;

                CheckIfEnded();
                currPlayer = (short)((currPlayer + 1) % 2);
            }
        }

        private bool CheckIfEnded()
        {
            if(visibleBoxes.Count < 2)
            {
                ShowPopupEnding(players[currPlayer].endingMessage);
                ResetGame_Click(null, null);
                return true;
            }
            return false;
        }
        private int FindPictureBoxIndex(PictureBox pictureBox)
        {
            short i = 0;
            while (pictureBoxes[i] != pictureBox) i++;
            return i;
        }

        private static void ShowPopupEnding(string message)
        {
            PopupWhenEnded popup = new PopupWhenEnded(message);
            DialogResult dialogresult = popup.ShowDialog();
            if (dialogresult == DialogResult.Cancel)
            {
                Application.Exit();
            }
            popup.Dispose();
        }

        private void ShowPopupChooseEnemy()
        {
            PopupChooseEnemy popup = new PopupChooseEnemy();
            DialogResult dialogresult = popup.ShowDialog();
            if (dialogresult == DialogResult.Cancel)
            {
                players[1] = new Player("Shrek", "Shrek: ", Label_PointsP2, "Shrek Wins!");
                player2_avatar.Image = (Image)Properties.Resources.ResourceManager.GetObject("shrek_avatar");
            }
            else if (dialogresult == DialogResult.OK)
            {
                players[1] = new Player("Donkey", "Donkey: ", Label_PointsP2, "Donkey Wins!");
                player2_avatar.Image = (Image)Properties.Resources.ResourceManager.GetObject("player2avat");
            }
            else Application.Exit();
            popup.Dispose();
        }
    }

    class Player
    {
        public short points;
        public string type;
        public Label pointsLabel;
        public string labeltext;
        public string endingMessage;

        public Player(string type, string labeltext, Label label, string endmessage)
        {
            this.type = type;
            this.labeltext = labeltext;
            pointsLabel = label;
            endingMessage = endmessage;
            ChangePoints(0);
        }

        public void ChangePoints(short points)
        {
            this.points = points;
            pointsLabel.Text = labeltext + Convert.ToString(points);
        }
    }
}
