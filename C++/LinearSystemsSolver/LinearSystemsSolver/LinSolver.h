// LinSolver.h
// Both declarations and definitions of all the linear equation system solver functions and decomposition functions

#pragma once
#include<vector>

#include "Matrix.h"

// System Solver Exceptions
// thrown when errors occur when solving the system
// for eg. when the system is not solvable, or when the system is not square
class SystemSolverException : public LinSolveBaseException {
public:
	SystemSolverException(const std::string& message) : e_message(message) {}
	virtual const char* what() const throw() { return e_message.c_str(); }
private:
	std::string e_message;
};

class LinSolver {
public:
	template<Numerical T>
	static Matrix<T> solve_elimination(const Matrix<T>& system);
	template<Numerical T>
	static Matrix<T> solve_lu(const Matrix<T>& system);
	template<Numerical T>
	static Matrix<T> solve_gauss_seidel(const Matrix<T>& system, const int max_steps = 10000, const T accuracy = T(0));
	template<Numerical_WithSqrt T>
	static Matrix<T> solve_qr(const Matrix<T>& system);
	template<Numerical T>
	static void LU_decompose(Matrix<T>& input, Matrix<T>& lower, Matrix<T>& upper, Matrix<T>& b);
	template<Numerical T>
	static Matrix<T> permutation_vector(const int size);
	template<Numerical T>
	static Matrix<T> permuation_vector_to_matrix(const Matrix<T>& p_vector);
	template<Numerical_WithSqrt T>
	static void QR_decompose(const Matrix<T>& input, Matrix<T>& q, Matrix<T>& r);
private:
	template<Numerical T>
	static Matrix<T> forward_substitution(const Matrix<T>& matrix, const Matrix<T>& b);
	template<Numerical T>
	static Matrix<T> back_substitution(const Matrix<T>& matrix, const Matrix<T>& b);
	template<Numerical T>
	static void divide_system(const Matrix<T>& input, Matrix<T>& left, Matrix<T>& right);
	template<Numerical T>
	static int get_row_to_switch(Matrix<T>& input, const int column_idx);
	template<Numerical T>
	static void switch_rows(Matrix<T>& input, const int idx1, const int idx2);
	template<Numerical T>
	static void split_lu(const Matrix<T>& input, Matrix<T>& lower, Matrix<T>& upper);
	template<Numerical T>
	static bool gs_check_accuracy(const Matrix<T>& old_x, const Matrix<T>& new_x, T accuracy);
	template<Numerical_WithSqrt T>
	static T dot_product(const std::vector<T>& x, const std::vector<T>& y);
	template<Numerical_WithSqrt T>
	static T vector_norm(const std::vector<T>& x);
	template<Numerical_WithSqrt T>
	static Matrix<T> householder_reflection(const std::vector<T>& x);
};

template<Numerical T>
Matrix<T> LinSolver::solve_lu(const Matrix<T>& system) {
	Matrix<T> left, lower, upper, b;
	divide_system(system, left, b);

	LU_decompose(left, lower, upper, b);
	Matrix<T> temp = forward_substitution(lower, b);
	Matrix<T> result = back_substitution(upper, temp);
	return result;
}

template<Numerical T>
Matrix<T> LinSolver::solve_elimination(const Matrix<T>& system)
{
	Matrix<T> matrix, b;
	divide_system(system, matrix, b);
	if(!matrix.is_square())
		throw SystemSolverException("Error: invalid linear equation system format, input matrix is not square");

	int row_count = matrix.get_row_count();
	for (int i = 0; i < row_count; i++)
		for (int j = i + 1; j < row_count; j++)
		{
			T val1 = -matrix[j][i];
			T val2 = matrix[i][i];
			matrix[j][i] = 0;
			for (int k = i + 1; k < row_count; k++)
				matrix[j][k] = val1 * matrix[i][k] + val2 * matrix[j][k];
			b(j) = val1 * b(i) + val2 * b(j);
		}

	return back_substitution(matrix, b);
}

template<Numerical T>
Matrix<T> LinSolver::solve_gauss_seidel(const Matrix<T>& system, const int max_steps, const T accuracy)
{
	Matrix<T> matrix, b;
	divide_system(system, matrix, b);
	int row_count = matrix.get_row_count();
	Matrix<T> x(row_count, 1);
	
	if (!matrix.is_square())
		throw SystemSolverException("Error: invalid linear equation system format, input matrix is not square");
	for (int i = 0; i < row_count; i++)
		if(matrix[i][i] == 0)
			throw SystemSolverException("Error: cannot compute Gauss Seidel algorithm, zero on the input matrixs diagonal");

	for (int step = 0; step < max_steps; step++)
	{
		Matrix<T> old_x(row_count, 1);
		for (int i = 0; i < row_count; i++)
			old_x(i) = x(i);
		for (int i = 0; i < row_count; i++)
		{
			T dot = 0;
			for (int j = 0; j < row_count; j++)
				if (i != j) dot = dot + matrix[i][j] * x(j);
			x(i) = (b(i) - dot) / matrix[i][i];
		}

		if (gs_check_accuracy(old_x, x, accuracy))
			return x;
	}

	return x;
}

template<Numerical_WithSqrt T>
Matrix<T> LinSolver::solve_qr(const Matrix<T>& system)
{
	Matrix<T> left, b, q, r;
	divide_system(system, left, b);
	QR_decompose(left, q, r);

	auto y = q.transpose() * b;
	Matrix<T> result = back_substitution(r, y);
	return result;
}

template<Numerical T>
void LinSolver::LU_decompose(Matrix<T>& input, Matrix<T>& lower, Matrix<T>& upper, Matrix<T>& b)
{
	if (!input.is_square())
		throw SystemSolverException("Error: cannot LU decompose input matrix, input matrix is not square");

	int num_rows = input.get_row_count();
	for (int i = 0; i < num_rows; i++) {
		int max_row = get_row_to_switch(input, i);
		if (max_row != i) {
			switch_rows(input, i, max_row);
			switch_rows(b, i, max_row);
		}
		for (int j = i + 1; j < num_rows; j++)
		{
			input[j][i] = input[j][i] / input[i][i];
			for (int k = i + 1; k < num_rows; k++)
				input[j][k] = input[j][k] - input[j][i] * input[i][k];
		}
	}

	split_lu(input, lower, upper);
}

// Returns the permutation vector of size n
// Used in LU decomposition to store the row switches
// Used only when calculating LU decomposition outside the solve_lu function, the permutation vector is given instead of the b vector to the decomposition function
template<Numerical T>
Matrix<T> LinSolver::permutation_vector(const int size)
{
	Matrix<T> p_vec(size, 1);
	for (int i = 0; i < size; i++)
		p_vec(i) = i;
	return p_vec;
}

// Converts the permutation vector to a permutation matrix P such that P * A = L * U
template<Numerical T>
Matrix<T> LinSolver::permuation_vector_to_matrix(const Matrix<T>& p_vector)
{
	Matrix<T> p_matrix(p_vector.get_row_count(), p_vector.get_row_count());
	for (int i = 0; i < p_vector.get_row_count(); i++)
		p_matrix(i, p_vector(i)) = 1;
	return p_matrix;
}

template<Numerical_WithSqrt T>
void LinSolver::QR_decompose(const Matrix<T>& input, Matrix<T>& q, Matrix<T>& r)
{
	if (!input.is_square())
		throw SystemSolverException("Error: invalid linear equation system format, input matrix is not square");

	int num_rows = input.get_row_count();

	q = Matrix<T>::identity(num_rows);
	r.copy_from(input);

	for (int i = 0; i < num_rows; i++)
	{
		std::vector<T> x;
		for (int j = i; j < num_rows; j++)
			x.push_back(r.get_value(j, i));

		T norm = vector_norm(x);
		if (x[0] - norm == 0) break;
		x[0] = x[0] - norm;

		auto H_x = householder_reflection(x);
		auto H = Matrix<T>::identity(num_rows);
		for (int j = i; j < num_rows; j++)
			for (int k = i; k < num_rows; k++)
				H[j][k] = H_x[j - i][k - i];

		r = H * r;
		q = q * H;
	}
}

// Forward substitution - used in LU decomposition
template<Numerical T>
Matrix<T> LinSolver::forward_substitution(const Matrix<T>& matrix, const Matrix<T>& b)
{
	int row_count = matrix.get_row_count();
	Matrix<T> x(row_count, 1);
	for(int i = 0; i < row_count; i++)
	{
		if(matrix[i][i] == 0)
			throw SystemSolverException("Error: cannot compute forward substituion, zero on the matrixs diagonal");
		T curr_sum = 0;
		for (int j = 0; j < i; j++)
			curr_sum = curr_sum + matrix[i][j] * x(j);
		x(i) = (b(i) - curr_sum) / matrix[i][i];
	}
	return x;
}

// Backward substitution - used in LU decomposition, QR decomposition and Gauss-Seidel
template<Numerical T>
Matrix<T> LinSolver::back_substitution(const Matrix<T>& matrix, const Matrix<T>& b)
{
	int row_count = matrix.get_row_count();
	Matrix<T> x(row_count, 1);
	for (int i = row_count - 1; i >= 0; i--)
	{
		if (matrix[i][i] == 0)
			b(i) == 0 ?
				throw SystemSolverException("Error: cannot compute back substituion, infinitely many solutions or unable to find solution") :
				throw SystemSolverException("Error: cannot compute back substituion, no solution or unable to find solution");

		T curr_sum = 0;
		for (int j = i+1; j < row_count; j++)
			curr_sum = curr_sum + matrix[i][j] * x(j);
		x(i) = (b(i) - curr_sum) / matrix[i][i];
	}
	return x;
}

// Divides the input matrix into two matrices, the left matrix is the matrix without the last column and the right matrix is the last column ([A|b] -> A, b)
template<Numerical T>
void LinSolver::divide_system(const Matrix<T>& input, Matrix<T>& left, Matrix<T>& right)
{
	int row_count = input.get_row_count();
	int column_count = input.get_column_count();
	left.resize(row_count, column_count - 1);
	right.resize(row_count, 1);
	for (int i = 0; i < row_count; i++)
	{
		left[i].assign(input[i].cbegin(), input[i].cend() - 1);
		right[i][0] = input[i][column_count - 1];
	}
}

// Returns the index of the row with the biggest absolute value in the given column
// Used for partial pivotation in LU decomposition
template<Numerical T>
int LinSolver::get_row_to_switch(Matrix<T>& input, const int column_idx)
{
	int num_rows = input.get_row_count();
	T max_value = input[0][column_idx];
	int max_row = column_idx;
	for (int row_idx = column_idx + 1; row_idx < num_rows; row_idx++)
		if (max_value < abs(input[row_idx][column_idx])) {
			max_value = abs(input[row_idx][column_idx]);
			max_row = row_idx;
		}
	return max_row;
}

template<Numerical T>
void LinSolver::switch_rows(Matrix<T>& input, const int idx1, const int idx2)
{
	auto temp = input.get_row(idx1);
	input.set_row(idx1, input.get_row(idx2));
	input.set_row(idx2, temp);
}

// Splits the input matrix into two matrices, the left matrix is the lower triangular matrix and the right matrix is the upper triangular matrix
template<Numerical T>
void LinSolver::split_lu(const Matrix<T>& input, Matrix<T>& lower, Matrix<T>& upper)
{
	int row_count = input.get_row_count();
	lower.resize(row_count, row_count);
	upper.resize(row_count, row_count);

	for (int i = 0; i < row_count; i++)
	{
		lower[i][i] = 1;
		upper[i][i] = input[i][i];
		for (int j = i+1; j < row_count; j++)
		{
			lower[j][i] = input[j][i];
			upper[i][j] = input[i][j];
		}
	}
}

// Checks if current and previous iteration of Gauss-Seidel method are close enough to each other
template<Numerical T>
bool LinSolver::gs_check_accuracy(const Matrix<T>& old_x, const Matrix<T>& new_x, const T accuracy)
{
	for (int i = 0; i < old_x.get_row_count(); i++)
		if (abs(old_x(i) - new_x(i)) > accuracy)
			return false;
	return true;
}

template<Numerical_WithSqrt T>
T LinSolver::dot_product(const std::vector<T>& x, const std::vector<T>& y)
{
	T dot = 0;
	for (size_t i = 0; i < x.size(); i++)
		dot = dot + x[i] * y[i];
	return dot;
}

template<Numerical_WithSqrt T>
T LinSolver::vector_norm(const std::vector<T>& x)
{
	return sqrt(dot_product(x,x));
}

// Returns the householder matrix for the given vector
// Used in QR decomposition
template<Numerical_WithSqrt T>
Matrix<T> LinSolver::householder_reflection(const std::vector<T>& x)
{
	Matrix<T> householder = Matrix<T>::identity(x.size());

	Matrix<T> x_matrix = Matrix<T>(x.size(), 1);
	for (size_t i = 0; i < x.size(); i++)
		x_matrix[i][0] = x[i];

	Matrix<T> outer_product = x_matrix * x_matrix.transpose();
	T factor = T(2) / dot_product(x, x);
	for (size_t i = 0; i < x.size(); i++)
		for (size_t j = 0; j < x.size(); j++)
		{
			outer_product[i][j] = outer_product[i][j] * factor;
			householder[i][j] = householder[i][j] - outer_product[i][j];
		}

	return householder;
}
