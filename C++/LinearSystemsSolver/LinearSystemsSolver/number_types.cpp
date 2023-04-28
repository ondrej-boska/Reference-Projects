// number_types.cpp
// Definitions of functions for the Fraction type (FiniteGroup type has it's definitions in the header file since it is templated).

#include<numeric>
#include<cctype>

#include "number_types.h"

using namespace std;

Fraction& Fraction::operator=(const Fraction& other)
{
	numerator = other.numerator;
	denominator = other.denominator;
	return *this;
}

Fraction Fraction::operator-() const {
	return Fraction(-numerator, denominator);
}

// After all operations are done, the fraction is normalized to improve numerical stability

Fraction Fraction::operator+(const Fraction other) const {
	Fraction toReturn = Fraction(numerator * other.denominator + other.numerator * denominator, denominator * other.denominator);
	toReturn.normalize();
	return toReturn;
}
Fraction Fraction::operator-(const Fraction other) const {
	auto toReturn = Fraction(numerator, denominator) + -other;
	toReturn.normalize();
	return toReturn;
}
Fraction Fraction::operator*(const Fraction other) const {
	auto toReturn = Fraction(numerator * other.numerator, denominator * other.denominator);
	toReturn.normalize();
	return toReturn;
}
Fraction Fraction::operator/(const Fraction other) const {
	if (other.numerator == 0)
		throw NumberTypeException("Error when dividing two fractions: dividing by zero.");

	auto toReturn = Fraction(numerator, denominator) * Fraction(other.denominator, other.numerator);
	toReturn.normalize();
	return toReturn;
}
Fraction Fraction::operator+(const int other) const
{
	return Fraction(numerator + denominator * other, denominator);
}
Fraction Fraction::operator-(const int other) const
{
	return Fraction(numerator - denominator * other, denominator);
}
Fraction Fraction::operator*(const int other) const
{
	return Fraction(numerator * other, denominator);
}
Fraction Fraction::operator/(const int other) const
{
	return Fraction(numerator, denominator * other);
}

// normalizing the fraction, using gcd algorithm from standard library
void Fraction::normalize() {
	int divider = gcd(numerator, denominator);
	numerator /= divider;
	denominator /= divider;
}

void Fraction::print(std::ostream& stream)
{
	stream << *this;
}

// loads a fraction from an input stream
// the fraction can be in the form of a/b or a (implying denominator is equal to 1)
istream& operator>>(istream& input, Fraction& fraction) {
	input >> fraction.numerator;
	char slash;
	input.get(slash);
	if (isspace(slash))
		fraction.denominator = 1;
	else if (slash == '/')
		input >> fraction.denominator;
	else
		throw NumberTypeException("Error: cannot read fraction from input stream.");

	if(!fraction.is_valid())
		throw NumberTypeException("Error: cannot create fraction, invalid format.");
	return input;
}

// outputs a fraction to an output stream
// if denominator is equal to 1, the fraction is outputted as a single number
ostream& operator<<(ostream& output, Fraction fraction) {
	fraction.normalize();
	if (fraction.denominator == 1)
		output << fraction.numerator;
	else
		output << fraction.numerator << '/' << fraction.denominator;
	return output;
}
