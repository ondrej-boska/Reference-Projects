#include<complex>

auto operator<=> (const std::complex<double> left, const std::complex<double> right) {
	return abs(left) <=> abs(right);
}
bool operator== (const std::complex<double> left, const int right) {
	return left.real() == right && left.imag() == 0;
}