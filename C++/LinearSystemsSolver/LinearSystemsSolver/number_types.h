// number_types.h
// defines custom Fraction and FiniteGroup types

#pragma once

#include<compare>
#include<iostream>
#include<complex>

#include "Matrix.h"

// NumberTypeException: exception thrown by operations done on Fraction and FiniteGroup types
// eg. when dividing by zero, or creating a fraction with denominator 0
class NumberTypeException : public LinSolveBaseException {
public:
	NumberTypeException(const std::string& message) : e_message(message) {}
	virtual const char* what() const throw() { return e_message.c_str(); }
private:
	std::string e_message;
};;

struct Fraction {
public:
	Fraction() : numerator(0), denominator(1) {}
	Fraction(long numerator, unsigned long denominator = 1) : numerator(numerator), denominator(denominator) {
		if (!is_valid())
			throw NumberTypeException("Error: cannot create fraction, invalid format.");
	}
	Fraction(const Fraction& other) : numerator(other.numerator), denominator(other.denominator) {}
	
	Fraction& operator=(const Fraction& other);
	Fraction operator-() const;

	Fraction operator+(const Fraction other) const;
	Fraction operator-(const Fraction other) const;
	Fraction operator*(const Fraction other) const;
	Fraction operator/(const Fraction other) const;

	Fraction operator+(const int other) const;
	Fraction operator-(const int other) const;
	Fraction operator*(const int other) const;
	Fraction operator/(const int other) const;

	operator double() { return static_cast<double>(numerator) / static_cast<double>(denominator); }
	void normalize();

	friend Fraction abs(const Fraction fraction) { return Fraction(abs(fraction.numerator), fraction.denominator); }
	void print(std::ostream& stream = std::cout);

	auto operator<=>(const Fraction other) const {
		return numerator * other.denominator <=> denominator * other.numerator;
	}
	bool operator==(const Fraction other) const {
		return numerator * other.denominator == denominator * other.numerator;
	}
	bool operator!=(const Fraction other) const {
		return numerator * other.denominator != denominator * other.numerator;
	}
	bool operator==(const int other) {
		this->normalize();
		return other == numerator && denominator == 1;
	}
	bool operator!=(const int other) {
		this->normalize();
		return other != numerator || denominator != 1;
	}

	friend std::istream& operator>>(std::istream& input, Fraction& fraction);
	friend std::ostream& operator<<(std::ostream& output, const Fraction fraction);
private:
	long numerator;
	unsigned long denominator;
	bool is_valid() {
		return denominator != 0;
	}
};

template<int N>
struct FiniteGroup {
public:
	FiniteGroup() : _value(0) {}
	FiniteGroup(int value) : _value(((value%N)+N)%N) { }

	FiniteGroup operator+(const FiniteGroup<N> other) const { return FiniteGroup<N>(_value + other._value); }
	FiniteGroup operator-(const FiniteGroup<N> other) const { return FiniteGroup<N>(_value - other._value + N); }
	FiniteGroup operator*(const FiniteGroup<N> other) const { return FiniteGroup<N>(_value * other._value); }
	FiniteGroup operator/(const FiniteGroup<N> other) const { return FiniteGroup<N>(_value * other.inverse()._value); }

	bool operator==(const FiniteGroup<N> other) const { return _value == other._value; }
	bool operator!=(const FiniteGroup<N> other) const { return _value != other._value; }
	auto operator<=>(const FiniteGroup<N>& other) const {
		return _value <=> other._value;
	}

	// checks for equality with an int by converting the int to a FiniteGroup
	bool operator==(const int other) const { return _value == ((other%N)+N)%N; }
	bool operator!=(const int other) const { return _value != ((other%N)+N)%N; }

	friend FiniteGroup<N> abs(const FiniteGroup<N> val) { return FiniteGroup<N>(val._value); }
	FiniteGroup<N> operator-() const { return FiniteGroup<N>(N - _value); }

	friend std::istream& operator>>(std::istream& input, FiniteGroup& val) {
		int value;
		input >> value;
		val = FiniteGroup<N>(value);	
		return input;
	}
	friend std::ostream& operator<<(std::ostream& output, const FiniteGroup val) {
		output << val._value;
		return output;
	}

private:
	int _value;

	// finds the greatest common divider and values a, b such that a*x + b*y = gcd(a, b)
	// used to find the modular inverse of a number
	static int extended_gcd(int a, int b, int& x, int& y) {
		if (a == 0) {
			x = 0; y = 1; return b;
		}
		int x_temp, y_temp;
		int gcd = extended_gcd(b % a, a, x_temp, y_temp);
		x = y_temp - (b / a) * x_temp;
		y = x_temp;
		return gcd;
	}

	FiniteGroup<N> inverse() const {
		int gcd, x, y;
		gcd = extended_gcd(_value, N, x, y);
		if (gcd != 1)
			throw NumberTypeException("Error: modular inverse does not exist.");
		return FiniteGroup<N>(x + N);
	}
};

