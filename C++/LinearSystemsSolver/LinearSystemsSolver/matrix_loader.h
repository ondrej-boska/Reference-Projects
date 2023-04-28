// matrix_loader.h
// Basic functions enabling simple importing of matrices from streams like std::cin or std::ifstream

#pragma once

#include<iostream>
#include<string>

#include "Matrix.h"
#include "number_types.h"

class matrix_loader
{
public:
	// Loads matrix from stream in format:
	// row_count column_count
	// row_1_column_1 row_1_column_2 ... row_1_column_n
	// row_2_column_1 row_2_column_2 ... row_2_column_n
	// ...
	// row_n_column_1 row_n_column_2 ... row_n_column_n
	template<Numerical T>
	static Matrix<T> load_from_stream(std::istream& input = std::cin) {
		int row_count, column_count;
		input >> row_count;
		input >> column_count;

		Matrix<T> matrix(row_count, column_count);
		
		for (int i = 0; i < row_count; i++)
			for (int j = 0; j < column_count; j++)
				input >> matrix[i][j];

		return matrix;
	}

	// Loads matrix from stream in formatload
	// row_count column_count to_read
	// row_1 column_1 value_1
	// row_2 column_2 value_2
	// ...
	// row_n column_n value_n
	template<Numerical T>
	static Matrix<T> load_compact(std::istream& input = std::cin) {
		int row_count, column_count, to_read;
		input >> row_count;
		input >> column_count;
		input >> to_read;

		Matrix<T> matrix(row_count, column_count);
		int curr_row, curr_col;
		T val;

		for (int i = 0; i < to_read; i++)
		{
			input >> curr_row;
			input >> curr_col;
			input >> matrix[curr_row][curr_col];
		}

		return matrix;
	}
};