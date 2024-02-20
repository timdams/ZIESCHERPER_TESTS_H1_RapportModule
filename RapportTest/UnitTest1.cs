using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RapportTest
{
    [TestClass]
    public class UnitTest1
    {
        private void GraadTest(int graad, string text)
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                TestDryRun.Rapport r = new TestDryRun.Rapport();
                if (r.GetType().GetProperty("Percentage") != null)
                {
                    var p = r.GetType().GetProperty("Percentage");
                    p.SetValue(r, graad);
                    if (r.GetType().GetMethod("PrintGraad") != null)
                    {
                        var l = r.GetType().GetMethod("PrintGraad");
                        l.Invoke(r, null);
                        var result = sw.ToString().Trim().ToLower();
                        if (result == string.Empty)
                            Assert.Fail($"De methode PrintGraad moet zaken naar het scherm sturen, maar ik heb niets zien verschijnen op het scherm.{graad} zou {text} moeten geven maar ik kreeg {result} ");
                        Assert.IsTrue(result.Contains(text.ToLower()),$"{graad} zou {text} moeten geven maar ik kreeg {result}");
                    }
                    else
                    {
                        Assert.Fail("Geen methode PrintGraad gevonden");
                    }

                }
                else
                {
                    Assert.Fail("Geen autoproperty Percentage gevonden");
                }
            }
        }


        [TestMethod]
        public void PrintGraadTest()
        {
            GraadTest(10, "Niet geslaagd");
            GraadTest(40, "Niet geslaagd");
            GraadTest(50, "Voldoende");
            GraadTest(65, "Voldoende");
            GraadTest(68, "Voldoende");
            GraadTest(69, "Onderscheiding");
            GraadTest(75, "Onderscheiding");
            GraadTest(76, "Grote onderscheiding");
            GraadTest(85, "Grote onderscheiding");
            GraadTest(86, "Grootste onderscheiding");
        }
      

        /// <summary>
        /// Controleert of autoproperty percentage aanwezig is
        /// </summary>
        [TestMethod, Description("Controleert of autoproperty percentage aanwezig is")]
        public void PropPercentageTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                TestDryRun.Rapport r = new TestDryRun.Rapport();

                bool hasProp = r.GetType().GetProperty("Percentage") != null;
                Assert.AreEqual(true, hasProp,"Geen property Percentage gevonden");

                var isInt = r.GetType().GetProperty("Percentage").GetMethod.ReturnType;
                Assert.AreEqual(typeof(Int32), isInt, "Property Percentage niet van int type");


                var isAutoProp = IsAutoProp(r.GetType().GetProperty("Percentage"));
                Assert.AreEqual(isAutoProp, true, "Property Percentage geen autoprop.");

            }
        }
        // Bron: https://stackoverflow.com/questions/2210309/how-to-find-out-if-a-property-is-an-auto-implemented-property-with-reflection
        public bool IsAutoProp( PropertyInfo info)
        {
            bool mightBe = info.GetGetMethod()
                               .GetCustomAttributes(
                                   typeof(CompilerGeneratedAttribute),
                                   true
                               )
                               .Any();
            if (!mightBe)
            {
                return false;
            }


            bool maybe = info.DeclaringType
                             .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(f => f.Name.Contains(info.Name))
                             .Where(f => f.Name.Contains("BackingField"))
                             .Where(
                                 f => f.GetCustomAttributes(
                                     typeof(CompilerGeneratedAttribute),
                                     true
                                 ).Any()
                             )
                             .Any();

            return maybe;
        }
    }
}
