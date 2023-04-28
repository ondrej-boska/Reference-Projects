import sys

def nactisachovnici(soubor):
    global startx, starty
    f = open(soubor, 'r')
    sachovnice = [[]]*(12)
    sachovnice[0] = [-2]*(12)
    sachovnice[10] = [-2]*(12)
    sachovnice[1] = [-2]*(12)
    sachovnice[11] = [-2]*(12)
    for i in range(2, 10):
        radek = f.readline()
        sachovnice[i] = [-2, -2]
        for j in range(8):
            if radek[j] == '.':
                sachovnice[i].append(-1)
            elif radek[j] == 'X':
                sachovnice[i].append(-2)
            elif radek[j] == 'S':
                sachovnice[i].append(0)
                startx = i
                starty = j+2
            elif radek[j] == 'C':
                sachovnice[i].append(-3)
        sachovnice[i].append(-2)
        sachovnice[i].append(-2)
    return sachovnice


class vagon:
    def __init__(self, value, next = None):
        self.value = value
        self.next = next

class fronta:
    def __init__(self):
        self.head = None
        self.tail = None

    def insert(self, value):
        if self.head == None:
            self.head = self.tail = vagon(value)
        else: 
            self.tail.next = vagon(value)
            self.tail = self.tail.next

    def push(self):
        if(self.head == None):
            return
        temp = self.head.value
        self.head = self.head.next
        return temp

def FastHorse(sachovnice):
    queue = fronta()
    now = (startx, starty)
    tahy = ((1, 2), (-1, -2), (1, -2), (-1, 2), (2, 1), (-2, -1), (-2, 1), (2, -1))
    while(now != None):
        (tedx, tedy) = now
        for i in range(8):
            if(sachovnice[tedx+tahy[i][0]][tedy+tahy[i][1]] == -3):
                return sachovnice[tedx][tedy] + 1
            if(sachovnice[tedx+tahy[i][0]][tedy+tahy[i][1]] == -1):
                sachovnice[tedx+tahy[i][0]][tedy+tahy[i][1]] = sachovnice[tedx][tedy] + 1
                queue.insert((tedx+tahy[i][0], tedy+tahy[i][1]))
        if(queue.head != None): now = queue.push()
        else: now = None        
    return "NELZE"          

sachovnice = nactisachovnici("sach.txt")
ShortestPath = FastHorse(sachovnice)
print(ShortestPath)

