// Matrix.h
// Defines the Matrix<T> type used in all the algorithms implemented in LinSolver class
// Defines the Numerical concept
// Defines the LinSolveBaseException class from which all exceptions explicitly thrown by LinSolve library inherit

#pragma once
#include<vector>
#include<iostream>
#include<algorithm>
#include<string>
#include<exception>

// Numerical concept
// Used by all matrix and linsolver functions (apart from QR decomp.)
// Requires basic arithmetic operations, comparison operators, check for equality with int, abs(), unary minus and constructor from int
template<typename T>
concept Numerical = requires(T x, T y, int n) { x + y; x - y; x* y; x / y; abs(x); -x; x < y; x == n; x = n; T(n); };

// Numerical_WithSqrt: concept for numerical types that have sqrt function
// Used by the QR decomposition
template<typename T>
concept Numerical_WithSqrt = Numerical<T> && requires(T x) { std::sqrt(x); };

// All exceptions thrown by LinSolve library inherit from this class
class LinSolveBaseException : public std::exception {
public:
	virtual const char* what() const throw() = 0;
};

// Such exceptions are thrown when there are errors in basic matrix operations (incorrect dimensions when multiplying and adding)
class MatrixException : public LinSolveBaseException {
public:
	MatrixException(const std::string& message) : e_message(message) {}
	virtual const char* what() const throw() { return e_message.c_str(); }
private:
	std::string e_message;
};

template<Numerical T>
class Matrix {
public:
	Matrix() : _row_count(0), _column_count(0) { resize(_row_count, _column_count); }
	Matrix(int row_count, int column_count) : _row_count(row_count), _column_count(column_count) { resize(row_count, column_count); }

	Matrix(const Matrix<T>& other) : _row_count(other._row_count), _column_count(other._column_count) {
		_matrix = other._matrix;
	}
	Matrix(Matrix<T>&& other) : _row_count(other._row_count), _column_count(other._column_count) {
		_matrix = std::move(other._matrix);
	}

	Matrix<T>& operator=(const Matrix<T>& other) {
		_row_count = other._row_count;
		_column_count = other._column_count;
		_matrix = other._matrix;
		return *this;
	}

	Matrix<T>& operator=(Matrix<T>&& other) {
		_row_count = other._row_count;
		_column_count = other._column_count;
		_matrix = std::move(other._matrix);
		return *this;
	}

	static Matrix<T> identity(int size);

	void print(std::ostream& stream = std::cout) const;
	int get_row_count() const { return _row_count; }
	int get_column_count() const { return _column_count; }

	std::vector<T>& get_row(int idx) { 
		if (idx >= _row_count)
			throw MatrixException("Error: incorrect row index");
		return _matrix[idx]; 
	}
	const std::vector<T>& get_row(int idx) const { 
		if (idx >= _row_count)
			throw MatrixException("Error: incorrect row index");
		return _matrix[idx]; 
	}
	void set_row(int idx, std::vector<T>& row) { 
		if (row.size() != _column_count)
			throw MatrixException("Error: incorrect row size");
		_matrix[idx] = row; 
	}
	std::vector<T> get_column_copy(int idx) const;

	std::vector<T>& operator[](int idx) { return get_row(idx); }
	const std::vector<T>& operator[](int idx) const { return get_row(idx); }

	// access operator by (), especially useful for vectors represented by matricies
	T& operator()(int i, int j = 0) { return _matrix[i][j]; }
	const T& operator()(int i, int j = 0) const { return _matrix[i][j]; }

	T& get_value(int row, int column) { return _matrix[row][column]; }
	const T& get_value(int row, int column) const { return _matrix[row][column]; }

	void set_value(int row, int column, T value) { _matrix[row][column] = value; }

	Matrix<T> transpose() const;
	bool is_square() const { return _row_count == _column_count; }
	void resize(int row_count, int column_count);

	Matrix<T> operator+(const Matrix<T>& other);
	Matrix<T> operator*(const Matrix<T>& other);

	void copy_from(const Matrix<T>& source);

private:
	std::vector<std::vector<T>> _matrix;
	int _row_count;
	int _column_count;
};

// Creates an identity matrix of size x size
template<Numerical T>
Matrix<T> Matrix<T>::identity(int size)
{
	Matrix<T> identity(size, size);
	for (int i = 0; i < size; i++)
		identity[i][i] = 1;
	return identity;
}

// prints the matrix to given ostream, default is standard output
template<Numerical T>
void Matrix<T>::print(std::ostream& stream) const {
	for (auto&& row : _matrix) {
		std::copy(row.begin(), row.end(), std::ostream_iterator<T>(stream, " "));
		stream << std::endl;
	}
}

template<Numerical T>
std::vector<T> Matrix<T>::get_column_copy(int idx) const {
	if (idx >= _column_count)
		throw MatrixException("Error: incorrect column index");
	std::vector<T> column;
	for (auto&& row : _matrix)
		column.push_back(row[idx]);
	return column;
}

template<Numerical T>
Matrix<T> Matrix<T>::operator+(const Matrix<T>& other) {
	if (_column_count != other._column_count)
		throw MatrixException("Different number of columns");
	if (_row_count != other._row_count)
		throw MatrixException("Different number of rows");

	Matrix<T> sum(_row_count, _column_count);
	for (size_t i = 0; i < _row_count; i++)
		for (size_t j = 0; j < _column_count; j++)
			sum._matrix[i][j] = _matrix[i][j] + other._matrix[i][j];
	return sum;
}

// basic n^3 matrix multiplication
template<Numerical T>
Matrix<T> Matrix<T>::operator*(const Matrix<T>& other) {
	if (_column_count != other._row_count)
		throw MatrixException("Error when multiplying matricies: incompatible dimensions.");

	Matrix<T> product(_row_count, other._column_count);
	for (int i = 0; i < _row_count; i++)
		for (int j = 0; j < other._column_count; j++) 
		{
			T curr = {};
			for (int k = 0; k < _column_count; k++)
				curr = curr + _matrix[i][k] * other._matrix[k][j];
			product[i][j] = curr;
		}
	return product;
}

template<Numerical T>
void Matrix<T>::resize(int row_count, int column_count) {
	_row_count = row_count;
	_column_count = column_count;
	_matrix.resize(row_count);
	for (auto&& row : _matrix)
		row.resize(column_count);
}

template<Numerical T>
Matrix<T> Matrix<T>::transpose() const {
	Matrix<T> transposed(_column_count, _row_count);
	for (int i = 0; i < _row_count; i++)
		for (int j = 0; j < _column_count; j++)
			transposed[j][i] = _matrix[i][j];
	return transposed;
}

// copies the values from source to this matrix
template<Numerical T>
void Matrix<T>::copy_from(const Matrix<T>& source) {
	_row_count = source._row_count;
	_column_count = source._column_count;
	resize(_row_count, _column_count);
	for (int i = 0; i < _row_count; i++)
		for (int j = 0; j < _column_count; j++)
			_matrix[i][j] = source._matrix[i][j];
}