﻿/*
 * Range Solver Tests
 * ---------------------------------------------------------------------
 * Author: Geneva Smith (GenevaS)
 * Updated 2017/12/14
 * ---------------------------------------------------------------------
 */

using CompanionCubeCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests_CompanionCubeCalculator
{
    [TestClass]
    public class OperatorStructTests
    {
        [TestMethod]
        public void TestOperatorConstructor()
        {
            // unittest-operatordatastructureconstructor
            OperatorStruct op = new OperatorStruct("+", 1, false, true, false, true);

            Assert.AreEqual("+", op.GetOperator());
            Assert.AreEqual(1, op.GetPrecedence());
            Assert.AreEqual(false, op.IsUnary());
            Assert.AreEqual(true, op.IsBinary());
            Assert.AreEqual(false, op.IsTernary());
            Assert.AreEqual(true, op.IsLeftAssociative());
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "Error: Cannot have an operator with no representative symbol.")]
        public void TestMissingOpSymbol()
        {
            // unittest-operatordatastructuremissingsym
            OperatorStruct op = new OperatorStruct("", 1, false, true, false, true);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "Error: Precedence values must be greater than 0.")]
        public void TestLowOpPrecedence()
        {
            // unittest-operatordatastructurelowprecedence
            OperatorStruct op = new OperatorStruct("+", 0, false, true, false, true);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "Error: An operator cannot be overloaded to be unary, binary, and ternary.")]
        public void TestOpType1()
        {
            // unittest-operatordatastructureoverloadedtype1
            OperatorStruct op = new OperatorStruct("+", 1, true, true, false, true);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "Error: An operator cannot be overloaded to be unary, binary, and ternary.")]
        public void TestOpType2()
        {
            // unittest-operatordatastructureoverloadedtype2
            OperatorStruct op = new OperatorStruct("+", 1, false, true, true, true);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "Error: An operator cannot be overloaded to be unary, binary, and ternary.")]
        public void TestOpType3()
        {
            // unittest-operatordatastructureoverloadedtype3
            OperatorStruct op = new OperatorStruct("+", 1, true, false, true, true);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "Error: Operators must be assigned a number of operands type.")]
        public void TestNoOpType()
        {
            // unittest-operatordatastructurenotype
            OperatorStruct op = new OperatorStruct("+", 1, false, false, false, true);
        }
    }

    [TestClass]
    public class SolverTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "Error: No information was provided for the equation.")]
        public void TestNoEquation()
        {
            // unittest-solvernoequation
            Solver.FindRange(null, new IntervalStruct[] { new IntervalStruct("x", 1, 2, true, true) });
        }

        [TestMethod]
        public void TestUnknownOp()
        {
            // unittest-solverunknownop
            string varToken = EquationConversion.GetVariableToken();
            EquationStruct equation = new EquationStruct ("**", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "x", null, null));
            IntervalStruct[] intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 3, true, true) };

            IntervalStruct range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);
        }

        [TestMethod]
        public void TestMissingIntervals()
        {
            // unittest-solvermissingintervals
            string varToken = EquationConversion.GetVariableToken();
            EquationStruct equation = new EquationStruct(varToken, "x", null, null);
            IntervalStruct[] intervals = new IntervalStruct[] { };

            IntervalStruct range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);
        }

        [TestMethod]
        public void TestAtomicEquations()
        {
            string varToken = EquationConversion.GetVariableToken();
            string constToken = EquationConversion.GetConstToken();

            // unittest-solverconstantfunction
            EquationStruct equation = new EquationStruct(constToken, "42", null, null);
            IntervalStruct[] intervals = new IntervalStruct[] { };

            IntervalStruct range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(42, range.GetMinBound());
            Assert.AreEqual(42, range.GetMaxBound());

            // unittest-solvervariablefunction
            equation = new EquationStruct(varToken, "x", null, null);
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 3, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(2, range.GetMinBound());
            Assert.AreEqual(3, range.GetMaxBound());
        }

        [TestMethod]
        public void TestAdditionEquations()
        {
            string varToken = EquationConversion.GetVariableToken();
            string constToken = EquationConversion.GetConstToken();

            // test-calculate addition
            EquationStruct equation = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            IntervalStruct[] intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            IntervalStruct range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(5, range.GetMinBound());
            Assert.AreEqual(9, range.GetMaxBound());

            // test-calculate additionconstant
            equation = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "4", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 3, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(6, range.GetMinBound());
            Assert.AreEqual(7, range.GetMaxBound());

            // unittest-solveradditionleftsidenull
            equation = new EquationStruct("+", "", new EquationStruct("/", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), new EquationStruct(varToken, "x", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", -3, 5, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);
        }

        [TestMethod]
        public void TestSubtractionEquations()
        {
            string varToken = EquationConversion.GetVariableToken();
            string constToken = EquationConversion.GetConstToken();

            // test-calculate subtraction 
            EquationStruct equation = new EquationStruct("-", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            IntervalStruct[] intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            IntervalStruct range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(-1, range.GetMinBound());
            Assert.AreEqual(-1, range.GetMaxBound());

            // unittest-solversubtractionleftsidenull
            equation = new EquationStruct("-", "", new EquationStruct("/", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), new EquationStruct(varToken, "x", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", -3, 5, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);
        }

        [TestMethod]
        public void TestMultiplicationEquations()
        {
            string varToken = EquationConversion.GetVariableToken();
            string constToken = EquationConversion.GetConstToken();

            // test-calculate multiplication1
            EquationStruct equation = new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            IntervalStruct[] intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            IntervalStruct range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(6, range.GetMinBound());
            Assert.AreEqual(20, range.GetMaxBound());

            // test-calculate multiplication2
            equation = new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -1, 3, true, true), new IntervalStruct("y", -3, 5, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(-9, range.GetMinBound());
            Assert.AreEqual(15, range.GetMaxBound());

            // test-calculate multiplication3
            equation = new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -3, -1, true, true), new IntervalStruct("y", -5, -2, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(2, range.GetMinBound());
            Assert.AreEqual(15, range.GetMaxBound());

            // unittest-solvermultiplicationleftsidenull
            equation = new EquationStruct("*", "", new EquationStruct("/", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), new EquationStruct(varToken, "x", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", -3, 5, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);
        }

        [TestMethod]
        public void TestDivisionEquations()
        {
            string varToken = EquationConversion.GetVariableToken();
            string constToken = EquationConversion.GetConstToken();

            // test-calculate divisionPositiveDivisor1
            EquationStruct equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            IntervalStruct[] intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            IntervalStruct range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(2.0 / 5, range.GetMinBound());
            Assert.AreEqual(4.0 / 3, range.GetMaxBound());

            // test-calculate divisionPositiveDivisor2
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 0, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(0, range.GetMinBound());
            Assert.AreEqual(4.0 / 3, range.GetMaxBound());

            // test-calculate divisionPositiveDivisor3
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -1, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(-1.0 / 3, range.GetMinBound());
            Assert.AreEqual(4.0 / 3, range.GetMaxBound());

            // test-calculate divisionPositiveDivisor4
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -2, -1, true, true), new IntervalStruct("y", 3, 5, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(-2.0 / 3, range.GetMinBound());
            Assert.AreEqual(-1.0 / 5, range.GetMaxBound());

            // test-calculate divisionPositiveDivisor5
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -2, 0, true, true), new IntervalStruct("y", 3, 5, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(-2.0 / 3, range.GetMinBound());
            Assert.AreEqual(0, range.GetMaxBound());

            // test-calculate divisionNegativeDivisor1
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", -5, -3, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(4.0 / -3, range.GetMinBound());
            Assert.AreEqual(2.0 / -5, range.GetMaxBound());

            // test-calculate divisionNegativeDivisor2
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 0, 4, true, true), new IntervalStruct("y", -5, -3, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(4.0 / -3, range.GetMinBound());
            Assert.AreEqual(0, range.GetMaxBound());

            // test-calculate divisionNegativeDivisor3
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -1, 4, true, true), new IntervalStruct("y", -5, -3, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(4.0 / -3, range.GetMinBound());
            Assert.AreEqual(-1.0 / -3, range.GetMaxBound());

            // test-calculate divisionNegativeDivisor4
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -2, 0, true, true), new IntervalStruct("y", -5, -3, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(0, range.GetMinBound());
            Assert.AreEqual(-2.0 / -3, range.GetMaxBound());

            // test-calculate divisionNegativeDivisor5
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -2, -1, true, true), new IntervalStruct("y", -5, -3, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(-1.0 / -5, range.GetMinBound());
            Assert.AreEqual(-2.0 / -3, range.GetMaxBound());

            // test-calculate divisionbyzero
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "0", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);

            // test-calculate divisionMixedIntervalDivisor 
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", -3, 5, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);

            // test-calculate divisionMixedIntervalDivisorZero1
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", -3, 0, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);

            // test-calculate divisionMixedIntervalDivisorZero2
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 0, 3, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);

            // test-calculate mixedDivisorComponent
            // Ideally, I would like this to return a partial answer
            equation = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct("()", "", new EquationStruct("*", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -2, -1, true, true), new IntervalStruct("y", 3, 5, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);

            // unittest-solverdivisionleftsidenull
            equation = new EquationStruct("/", "", new EquationStruct("/", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), new EquationStruct(varToken, "x", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", -3, 5, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);
        }

        [TestMethod]
        public void TestExponentEquations()
        {
            string varToken = EquationConversion.GetVariableToken();
            string constToken = EquationConversion.GetConstToken();

            // test-calculate intervalAsExponents
            EquationStruct equation = new EquationStruct("^", "", new EquationStruct(constToken, "2", null, null), new EquationStruct(varToken, "x", null, null));
            IntervalStruct[] intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true) };

            IntervalStruct range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(4, range.GetMinBound());
            Assert.AreEqual(16, range.GetMaxBound());

            // test-calculate intervalAsExponentsInvalidBase
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "1", null, null), new EquationStruct(constToken, "x", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -4, -2, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);

            // test-calculate intervalWithExponent1 
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "3", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(8, range.GetMinBound());
            Assert.AreEqual(64, range.GetMaxBound());

            // test-calculate intervalWithExponent2
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "2", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(4, range.GetMinBound());
            Assert.AreEqual(16, range.GetMaxBound());

            // test-calculate intervalWithExponent3
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "2", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 0, 4, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(0, range.GetMinBound());
            Assert.AreEqual(16, range.GetMaxBound());

            // test-calculate intervalWithExponent4
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "2", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -2, 4, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(0, range.GetMinBound());
            Assert.AreEqual(16, range.GetMaxBound());

            // test-calculate intervalWithExponent5
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "2", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", -4, -2, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(4, range.GetMinBound());
            Assert.AreEqual(16, range.GetMaxBound());

            // test-calculate intervalWithInvalidExponent1
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "2.1", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(4, range.GetMinBound());
            Assert.AreEqual(16, range.GetMaxBound());

            // test-calculate intervalWithInvalidExponent2
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "-1", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);

            // test-calculate\_intervalsOnlyExponentiation
            equation = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);

            // unittest-solverexponentleftsidenull
            equation = new EquationStruct("^", "", new EquationStruct("/", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), new EquationStruct(constToken, "4", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("y", -3, 5, true, true), new IntervalStruct("z", -1, 1, true, true) };

            range = Solver.FindRange(equation, intervals);

            Assert.AreEqual(null, range);
        }

        [TestMethod]
        public void TestPrecedenceOfOperations()
        {
            string varToken = EquationConversion.GetVariableToken();
            string constToken = EquationConversion.GetConstToken();
            
            // x+y−z == (x+y)−z
            EquationStruct equation1 = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct("-", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)));
            EquationStruct equation2 = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct("()", "", new EquationStruct("-", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), null));
            IntervalStruct[] intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true), new IntervalStruct("z", 2, 2, true, true) };

            IntervalStruct range1 = Solver.FindRange(equation1, intervals);
            IntervalStruct range2 = Solver.FindRange(equation2, intervals);

            Assert.AreEqual(range1.GetMinBound(), range2.GetMinBound());
            Assert.AreEqual(range1.GetMaxBound(), range2.GetMaxBound());

            // x∗y/z == (x∗y)/z
            equation1 = new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct("/", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)));
            equation2 = new EquationStruct("/", "", new EquationStruct("()", "", new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null)), null), new EquationStruct(varToken, "z", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true), new IntervalStruct("z", 2, 2, true, true) };

            range1 = Solver.FindRange(equation1, intervals);
            range2 = Solver.FindRange(equation2, intervals);

            Assert.AreEqual(range1.GetMinBound(), range2.GetMinBound());
            Assert.AreEqual(range1.GetMaxBound(), range2.GetMaxBound());

            // x + y∗z == x + (y∗z)
            equation1 = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct("*", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)));
            equation2 = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct("()", "", new EquationStruct("*", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true), new IntervalStruct("z", 2, 2, true, true) };

            range1 = Solver.FindRange(equation1, intervals);
            range2 = Solver.FindRange(equation2, intervals);

            Assert.AreEqual(range1.GetMinBound(), range2.GetMinBound());
            Assert.AreEqual(range1.GetMaxBound(), range2.GetMaxBound());

            // 2x ∗y == (2x)∗y
            equation1 = new EquationStruct("*", "", new EquationStruct(constToken, "2", null, null), new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null)));
            equation2 = new EquationStruct("*", "", new EquationStruct("()", "", new EquationStruct("*", "", new EquationStruct(constToken, "2", null, null), new EquationStruct(varToken, "x", null, null)), null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            range1 = Solver.FindRange(equation1, intervals);
            range2 = Solver.FindRange(equation2, intervals);

            Assert.AreEqual(range1.GetMinBound(), range2.GetMinBound());
            Assert.AreEqual(range1.GetMaxBound(), range2.GetMaxBound());

            // x2 ∗y == (x2)∗y
            equation1 = new EquationStruct("*", "", new EquationStruct(varToken, "x2", null, null), new EquationStruct(varToken, "y", null, null));
            equation2 = new EquationStruct("*", "", new EquationStruct("()", "", new EquationStruct(varToken, "x2", null, null), null), new EquationStruct(varToken, "y", null, null));
            intervals = new IntervalStruct[] { new IntervalStruct("x2", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true) };

            range1 = Solver.FindRange(equation1, intervals);
            range2 = Solver.FindRange(equation2, intervals);

            Assert.AreEqual(range1.GetMinBound(), range2.GetMinBound());
            Assert.AreEqual(range1.GetMaxBound(), range2.GetMaxBound());

            // x∗(y + z)
            equation1 = new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct("()", "", new EquationStruct("+", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), null));
            intervals = new IntervalStruct[] { new IntervalStruct("x", 2, 4, true, true), new IntervalStruct("y", 3, 5, true, true), new IntervalStruct("z", 2, 2, true, true) };

            range1 = Solver.FindRange(equation1, intervals);

            Assert.AreEqual(10, range1.GetMinBound());
            Assert.AreEqual(28, range1.GetMaxBound());
        }
    }
}