#pragma once

// complex_extensions.h
// Implements comparison and equality operators for complex numbers
// Needed for the complex<double> type to be compatible with the Numerical concept
// Note that complex numbers are not ordered and in this project are compared by their absolute value, which works well with pivoting in LU decomposition

#include<complex>

auto operator<=> (const std::complex<double> left, const std::complex<double> right) {
	return abs(left) <=> abs(right);
}
bool operator== (const std::complex<double> left, const int right) {
	return left.real() == right && left.imag() == 0;
}
