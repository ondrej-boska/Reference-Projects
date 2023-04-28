// Linear Equation System Solver
// Ondrej Boska
// Semester project for NPRG041

// main.cpp
// Test functions and usage examples for implemented algorithms 

// USAGE
// Set the test_type to the type you want to test the algorithms with (default: double)
// Run the program, enter one of the systems included in the attached .txt files

#include<vector>
#include<string>
#include<iostream>
#include<chrono>
#include<complex>
#include<type_traits>

#include "Matrix.h"
#include "number_types.h"
#include "matrix_loader.h"
#include "LinSolver.h"

#include "complex_extensions.h"

using namespace std;

// All tests are performed using defined type: double/Fraction/FiniteGroup/complex<double>
using test_type = std::complex<double>;

template<Numerical T>
void test_lu(Matrix<T> matrix) {
	Matrix<T> L, U;
	Matrix<T> perm = LinSolver::permutation_vector<test_type>(matrix.get_row_count());

	LinSolver::LU_decompose<T>(matrix, L, U, perm);

	cout << "==== L ====" << endl;
	L.print();
	cout << endl;
	cout << "==== U ====" << endl;
	U.print();
	cout << endl;
	cout << "==== P ====" << endl;
	perm.print();
	cout << endl;
	// Test by multiplication - multiplying L and U should result in original matrix
	cout << "==== TEST ====" << endl;
	(LinSolver::permuation_vector_to_matrix<T>(perm) * L * U).print();
	cout << endl;
}

template<Numerical T>
void test_qr(Matrix<T> matrix) {
	Matrix<T> Q, R;

	LinSolver::QR_decompose<T>(matrix, Q, R);

	cout << "==== Q ====" << endl;
	Q.print();
	cout << "==== R ====" << endl;
	R.print();
	cout << "==== TEST ====" << endl;
	(Q * R).print(); // should result in original matrix
	cout << endl;
}

template<typename T>
void test_lu_solve(Matrix<T> system) {
	cout << "==== LU ====" << endl;

	auto start_time = chrono::high_resolution_clock::now();
	auto lu = LinSolver::solve_lu(system);
	auto end_time = chrono::high_resolution_clock::now();

	lu.print();
	cout << "Time: " << chrono::duration_cast<chrono::microseconds>(end_time - start_time).count() << " microseconds" << endl;
	cout << endl;
}

template<typename T>
void test_elimination(Matrix<T> system) {
	cout << "==== Elimination ====" << endl;

	auto start_time = chrono::high_resolution_clock::now();
	auto elimination = LinSolver::solve_elimination(system);
	auto end_time = chrono::high_resolution_clock::now();

	elimination.print();
	cout << "Time: " << chrono::duration_cast<chrono::microseconds>(end_time - start_time).count() << " microseconds" << endl;
	cout << endl;
}

template<typename T>
void test_gauss_seidel(Matrix<T> system) {
	cout << "==== Gauss-Seidel ====" << endl;

	auto start_time = chrono::high_resolution_clock::now();

	//When 0 is used as precision for Fraction, during the computation, when the precision is reached, the program tries to create a fraction with 0 in denominator, which is not possible
	//To avoid such error when computing gauss-seidel on fractions, swap the following two lines 
	//auto gauss = LinSolver::solve_gauss_seidel(system, 500, Fraction(1, 100));
	auto gauss = LinSolver::solve_gauss_seidel(system, 5000, test_type());
	auto end_time = chrono::high_resolution_clock::now();

	gauss.print();
	cout << "Time: " << chrono::duration_cast<chrono::microseconds>(end_time - start_time).count() << " microseconds" << endl;
	cout << endl;
}

template<typename T>
void test_qr_solve(Matrix<T> system) {
	cout << "==== QR ====" << endl;

	auto start_time = chrono::high_resolution_clock::now();
	auto qr = LinSolver::solve_qr(system);
	auto end_time = chrono::high_resolution_clock::now();

	qr.print();
	cout << "Time: " << chrono::duration_cast<chrono::microseconds>(end_time - start_time).count() << " microseconds" << endl;
	cout << endl;
}

int main(int argc, char** argv) {
	try {
		cout << "Enter matrix in following format: " << endl << endl;
		cout << "row_count column_count" << endl;
		cout << "row_1" << endl;
		cout << "row_2" << endl;
		cout << "..." << endl;
		cout << "row_n" << endl << endl;

		auto system = matrix_loader::load_from_stream<test_type>();
		cout << endl;

		// Tests for system solvers
		// Input matrix must be of size n times n+1 (where the last column is made from the right sides of the equations)

		try {
			test_lu_solve(system);
		}
		catch (const exception& ex) {
			cout << ex.what() << endl << endl;
		}
		try {
			test_elimination(system);
		}
		catch (const exception& ex) {
			cout << ex.what() << endl << endl;
		}
		try {
			test_gauss_seidel(system);
		}
		catch (const exception& ex) {
			cout << ex.what() << endl << endl;
		}
		// QR decomposition does not work with fractions and finite groups
		// fractions and finite groups do not implement the sqrt() function
		try {
			test_qr_solve(system);
		}
		catch (const exception& ex) {
			cout << ex.what() << endl << endl;
		}

		// Tests for decompositions
		// Input matrix must be square

		//try {
		//	test_lu(system);
		//}
		//catch (const exception& ex) {
		//	cout << ex.what() << endl << endl;
		//}
		//try {
		//	test_qr(system);
		//}
		//catch (const exception& ex) {
		//	cout << ex.what() << endl << endl;
		//}
	}
	catch (const exception& ex) {
		cout << ex.what();
	}
	catch (...) {
		cout << "Unknown error";
	}
}