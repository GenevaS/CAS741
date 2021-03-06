﻿/*
 * Equation Conversion and Data Structure Tests
 * ---------------------------------------------------------------------
 * Author: Geneva Smith (GenevaS)
 * Updated 2017/12/12
 * ---------------------------------------------------------------------
 */

using CompanionCubeCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTests_CompanionCubeCalculator
{
    [TestClass]
    public class EquationStructTests
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException), "Error: Equation structures must be assigned an operator during initialization.")]
        public void TestEquationStructNoOperator()
        {
            // unittest-equationdatastructurenoop
            EquationStruct eq = new EquationStruct("", "", null, null);
        }

        [TestMethod]
        public void TestEquationStructConstructorWithNulls()
        {
            // unittest-equationdatastructureconstructnulls
            EquationStruct eq = new EquationStruct("+", "x", null, null);
            Assert.AreEqual("+", eq.GetOperator());
            Assert.AreEqual("x", eq.GetVariableName());
            Assert.AreEqual(null, eq.GetLeftOperand());
            Assert.AreEqual(null, eq.GetRightOperand());
        }

        [TestMethod]
        public void TestEquationStructConstructor()
        {
            // unittest-equationdatastructureconstruct
            EquationStruct eq = new EquationStruct("+", "x", new EquationStruct("VAR", "y", null, null), new EquationStruct("VAR", "z", null, null));
            Assert.AreEqual("+", eq.GetOperator());
            Assert.AreEqual("x", eq.GetVariableName());
            Assert.AreEqual("y", eq.GetLeftOperand().GetVariableName());
            Assert.AreEqual("z", eq.GetRightOperand().GetVariableName());
        }

        [TestMethod]
        public void TestEqSetVariableName()
        {
            // unittest-equationdatastructuresetvarname
            EquationStruct eq = new EquationStruct("+", "x", null, null);
            eq.SetVariableName("y");
            Assert.AreEqual("y", eq.GetVariableName());
        }

        [TestMethod]
        public void TestEqSetLeftOperand()
        {
            // unittest-equationdatastructuresetleftop
            EquationStruct eq = new EquationStruct("+", "x", null, null);
            eq.SetLeftOperand(new EquationStruct("VAR", "y", null, null));
            Assert.AreEqual("y", eq.GetLeftOperand().GetVariableName());
        }

        [TestMethod]
        public void TestEqSetRightOperand()
        {
            // unittest-equationdatastructuresetrightop
            EquationStruct eq = new EquationStruct("+", "x", null, null);
            eq.SetRightOperand(new EquationStruct("VAR", "z", null, null));
            Assert.AreEqual("z", eq.GetRightOperand().GetVariableName());
        }
    }

    [TestClass]
    public class EquationConversionTests
    {
        [TestMethod]
        public void TestConfigWithNoOperators()
        {
            OperatorStruct[] ops = new OperatorStruct[] { };

            EquationConversion.ResetEquationConversion();

            // unittest-equationconversionconfignoops
            Assert.AreEqual(false, EquationConversion.ConfigureParser(ops, Solver.GetValidTerminators()));
        }

        [TestMethod]
        public void TestConfigWithUnaryOperator()
        {
            OperatorStruct[] ops = new OperatorStruct[] { new OperatorStruct("-", 5, true, false, false, false) };

            EquationConversion.ResetEquationConversion();

            // unittest-equationconversionconfigunary
            Assert.AreEqual(true, EquationConversion.ConfigureParser(ops, Solver.GetValidTerminators()));
        }

        [TestMethod]
        public void TestConfigWithBinaryOperator()
        {
            OperatorStruct[] ops = new OperatorStruct[] { new OperatorStruct("-", 5, false, true, false, false) };

            EquationConversion.ResetEquationConversion();

            // unittest-equationconversionconfigbinary
            Assert.AreEqual(true, EquationConversion.ConfigureParser(ops, Solver.GetValidTerminators()));
        }

        [TestMethod]
        public void TestConfigWithTernaryOperator()
        {
            OperatorStruct[] ops = new OperatorStruct[] { new OperatorStruct("&", 5, false, false, true, false) };

            EquationConversion.ResetEquationConversion();

            // unittest-equationconversionconfigternary
            Assert.AreEqual(false, EquationConversion.ConfigureParser(ops, Solver.GetValidTerminators()));
        }

        [TestMethod]
        public void TestConfigWithUnBalancedLeftTerminator()
        {
            string[][] terminators = new string[][] { new string[] { "(", "" } };

            EquationConversion.ResetEquationConversion();

            // unittest-equationconversionconfigunbalancedleftterm
            Assert.AreEqual(false, EquationConversion.ConfigureParser(Solver.GetValidOperators(), terminators));
        }

        [TestMethod]
        public void TestConfigWithUnBalancedRightTerminator()
        {
            string[][] terminators = new string[][] { new string[] { "", ")" } };

            EquationConversion.ResetEquationConversion();

            // unittest-equationconversionconfigunbalancedrightterm
            Assert.AreEqual(false, EquationConversion.ConfigureParser(Solver.GetValidOperators(), terminators));
        }

        [TestMethod]
        public void TestUnaryFunction()
        {
            OperatorStruct[] ops = new OperatorStruct[] { new OperatorStruct("-", 5, true, false, false, false) };
            string varToken = EquationConversion.GetVariableToken();

            EquationConversion.ResetEquationConversion();
            EquationConversion.ConfigureParser(ops, Solver.GetValidTerminators());

            if (EquationConversion.IsReady())
            {
                string constToken = EquationConversion.GetConstToken();

                // unittest-equationconversionparseunary
                EquationStruct unaryEq = EquationConversion.MakeEquationTree("-x");
                EquationStruct targetStructure = new EquationStruct("-", "", new EquationStruct(varToken, "x", null, null), null);

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(unaryEq));
                //Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));
            }
            else
            {
                Assert.Fail("Equation Parser could not be initialized.");
            }
        }

        [TestMethod]
        public void TestConstantValueFunction()
        {
            EquationConversion.ResetEquationConversion();
            EquationConversion.ConfigureParser(Solver.GetValidOperators(), Solver.GetValidTerminators());

            if(EquationConversion.IsReady())
            {
                string constToken = EquationConversion.GetConstToken();

                // test-input functionAsConstant
                EquationStruct constEq = EquationConversion.MakeEquationTree("42");
                EquationStruct targetStructure = new EquationStruct(constToken, "42", null, null);

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(constEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { }, EquationConversion.GetVariableList()));
            }
            else
            {
                Assert.Fail("Equation Parser could not be initialized.");
            }
        }

        [TestMethod]
        public void TestEquationWithConstants()
        {
            EquationConversion.ResetEquationConversion();
            EquationConversion.ConfigureParser(Solver.GetValidOperators(), Solver.GetValidTerminators());

            if (EquationConversion.IsReady())
            {
                string constToken = EquationConversion.GetConstToken();
                string varToken = EquationConversion.GetVariableToken();

                // test-parse constantValue1 
                EquationStruct constEq = EquationConversion.MakeEquationTree("4+x");
                EquationStruct targetStructure = new EquationStruct("+", "", new EquationStruct(constToken, "4", null, null), new EquationStruct(varToken, "x", null, null));

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(constEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));

                // test-parse constantValue2
                constEq = EquationConversion.MakeEquationTree("-4+x");
                targetStructure = new EquationStruct("+", "", new EquationStruct(constToken, "-4", null, null), new EquationStruct(varToken, "x", null, null));

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(constEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));

                // test-parse constantValue3
                constEq = EquationConversion.MakeEquationTree("x/-4");
                targetStructure = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "-4", null, null));

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(constEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));

                // test-parse constantValue4
                constEq = EquationConversion.MakeEquationTree("x+4");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "4", null, null));

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(constEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));

                // test-parse constantValue5
                constEq = EquationConversion.MakeEquationTree("x+-4");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "-4", null, null));

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(constEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));

                // test-parse implicitMultiplication
                constEq = EquationConversion.MakeEquationTree("4x");
                targetStructure = new EquationStruct("*", "", new EquationStruct(constToken, "4", null, null), new EquationStruct(varToken, "x", null, null));

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(constEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));

                // test-parse\_constantValue6
                constEq = EquationConversion.MakeEquationTree("x+42");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "42", null, null));

                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(constEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));

            }
            else
            {
                Assert.Fail("Equation Parser could not be initialized.");
            }
        }

        [TestMethod]
        public void TestEqBrackets()
        {
            EquationConversion.ResetEquationConversion();
            EquationConversion.ConfigureParser(Solver.GetValidOperators(), Solver.GetValidTerminators());

            if (EquationConversion.IsReady())
            {
                string constToken = EquationConversion.GetConstToken();
                string varToken = EquationConversion.GetVariableToken();

                // test-parse brackets1 
                EquationStruct brackEq = EquationConversion.MakeEquationTree("x+(y-z)");
                EquationStruct targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct("()", "", new EquationStruct("-", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(brackEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x", "y", "z" }, EquationConversion.GetVariableList()));

                // test-parse brackets2 
                brackEq = EquationConversion.MakeEquationTree("(x*y)-2^z");
                EquationStruct comp1 = new EquationStruct("()", "", new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null)), null);
                EquationStruct comp2 = new EquationStruct("^", "", new EquationStruct(constToken, "2", null, null), new EquationStruct(varToken, "z", null, null));

                targetStructure = new EquationStruct("-", "", comp1, comp2);
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(brackEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x", "y", "z" }, EquationConversion.GetVariableList()));

                // test-parse brackets3
                brackEq = EquationConversion.MakeEquationTree("w*(x/(y+z))");

                comp1 = new EquationStruct("()", "", new EquationStruct("+", "", new EquationStruct(varToken, "y", null, null), new EquationStruct(varToken, "z", null, null)), null); ;
                comp2 = new EquationStruct("()", "", new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), comp1), null);

                targetStructure = new EquationStruct("*", "", new EquationStruct(varToken, "w", null, null), comp2);
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(brackEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "w", "x", "y", "z" }, EquationConversion.GetVariableList()));

                //  test-parse openRightBracket
                brackEq = EquationConversion.MakeEquationTree("x+(y-z");
                Assert.AreEqual(null, brackEq);

                // test-parse openLeftBracket
                brackEq = EquationConversion.MakeEquationTree("x+y-z)");
                Assert.AreEqual(null, brackEq);
            }
            else
            {
                Assert.Fail("Equation Parser could not be initialized.");
            }
        }

        [TestMethod]
        public void TestIncompleteFunction()
        {
            EquationConversion.ResetEquationConversion();
            EquationConversion.ConfigureParser(Solver.GetValidOperators(), Solver.GetValidTerminators());

            if (EquationConversion.IsReady())
            {
                // unittest-equationconversion missingFunctionValue1
                EquationStruct incompleteEq = EquationConversion.MakeEquationTree("x+");
                Assert.AreEqual(null, incompleteEq);

                // unittest-equationconversion missingFunctionValue2
                incompleteEq = EquationConversion.MakeEquationTree("*x");
                Assert.AreEqual(null, incompleteEq);

                // unittest-equationconversion missingFunctionValue3
                incompleteEq = EquationConversion.MakeEquationTree("x+*y");
                Assert.AreEqual(null, incompleteEq);
            }
            else
            {
                Assert.Fail("Equation Parser could not be initialized.");
            }
        }

        [TestMethod]
        public void TestVariableNames()
        {
            EquationConversion.ResetEquationConversion();
            EquationConversion.ConfigureParser(Solver.GetValidOperators(), Solver.GetValidTerminators());

            if (EquationConversion.IsReady())
            {
                string varToken = EquationConversion.GetVariableToken();

                // test-input simpleVariableName
                EquationStruct varEq = EquationConversion.MakeEquationTree("x+y");
                EquationStruct targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x", "y" }, EquationConversion.GetVariableList()));

                // test-input multiCharacterVariableName1
                varEq = EquationConversion.MakeEquationTree("x1+y");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x1", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x1", "y" }, EquationConversion.GetVariableList()));

                // test-input multiCharacterVariableName2
                varEq = EquationConversion.MakeEquationTree("x_+y");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x_", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x_", "y" }, EquationConversion.GetVariableList()));

                // test-input multiCharacterVariableName3
                varEq = EquationConversion.MakeEquationTree("x_1+y");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x_1", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x_1", "y" }, EquationConversion.GetVariableList()));

                // test-input multiCharacterVariableName4
                varEq = EquationConversion.MakeEquationTree("x_i+y");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x_i", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x_i", "y" }, EquationConversion.GetVariableList()));

                // test-input multiCharacterVariableName5
                varEq = EquationConversion.MakeEquationTree("x''+y");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x''", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x''", "y" }, EquationConversion.GetVariableList()));

                // test-input multiCharacterVariableName6
                varEq = EquationConversion.MakeEquationTree("xy+y");
                targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "xy", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "xy", "y" }, EquationConversion.GetVariableList()));
            }
            else
            {
                Assert.Fail("Equation Parser could not be initialized.");
            }
        }

        [TestMethod]
        public void TestSimpleFunctions()
        {
            EquationConversion.ResetEquationConversion();
            EquationConversion.ConfigureParser(Solver.GetValidOperators(), Solver.GetValidTerminators());

            if (EquationConversion.IsReady())
            {
                string varToken = EquationConversion.GetVariableToken();
                string constToken = EquationConversion.GetConstToken();

                // test-parse addition 
                EquationStruct varEq = EquationConversion.MakeEquationTree("x+y");
                EquationStruct targetStructure = new EquationStruct("+", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x", "y" }, EquationConversion.GetVariableList()));

                // test-parse subtraction
                varEq = EquationConversion.MakeEquationTree("x-y");
                 targetStructure = new EquationStruct("-", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x", "y" }, EquationConversion.GetVariableList()));

                // test-parse multiplication
                varEq = EquationConversion.MakeEquationTree("x*y");
                targetStructure = new EquationStruct("*", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x", "y" }, EquationConversion.GetVariableList()));

                // test-parse division
                varEq = EquationConversion.MakeEquationTree("x/y");
                targetStructure = new EquationStruct("/", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(varToken, "y", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x", "y" }, EquationConversion.GetVariableList()));

                // test-parse intervalexponents
                varEq = EquationConversion.MakeEquationTree("2^x");
                targetStructure = new EquationStruct("^", "", new EquationStruct(constToken, "2", null, null), new EquationStruct(varToken, "x", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));

                // test-parse intervalbase
                varEq = EquationConversion.MakeEquationTree("x^2");
                targetStructure = new EquationStruct("^", "", new EquationStruct(varToken, "x", null, null), new EquationStruct(constToken, "2", null, null));
                Assert.AreEqual(PrintEquation(targetStructure), PrintEquation(varEq));
                Assert.AreEqual(true, CheckVariableList(new string[] { "x" }, EquationConversion.GetVariableList()));
            }
            else
            {
                Assert.Fail("Equation Parser could not be initialized.");
            }
        }

        /* HELPER FUNCTIONS */
        private static string PrintEquation(EquationStruct node)
        {
            string equation = "";

            if (node.GetLeftOperand() == null)
            {
                equation = node.GetOperator() + ": " + node.GetVariableName();
            }
            else if (node.GetRightOperand() == null)
            {
                equation = node.GetOperator() + "(" + PrintEquation(node.GetLeftOperand()) + ")";
            }
            else
            {
                equation = node.GetOperator() + "(" + PrintEquation(node.GetLeftOperand()) + ", " + PrintEquation(node.GetRightOperand()) + ")";
            }

            return equation;
        }

        private static bool CheckVariableList(string[] expected, string[] produced)
        {
            List<int> indiciesExpected = new List<int> ();
            List<int> indiciesProduced = new List<int>();
            int index;
            bool success = true;

            for (int i = 0; i < expected.Length; i++)
            {
                index = System.Array.IndexOf(produced, expected[i]);
                if (index > -1)
                {
                    indiciesProduced.Add(index);
                    indiciesExpected.Add(i);
                }
            }

            if((indiciesExpected.Count != indiciesProduced.Count) && (indiciesExpected.Count != expected.Length))
            {
                success = false;
            }

            return success;
        }
    }
}