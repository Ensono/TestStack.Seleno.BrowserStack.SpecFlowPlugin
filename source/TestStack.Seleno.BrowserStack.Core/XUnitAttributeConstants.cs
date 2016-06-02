namespace TestStack.Seleno.BrowserStack.Core
{
    public  class XUnitAttributeConstants
    {
        // public static readonly string TESTFIXTURE_ATTR = "NUnit.Framework.TestFixtureAttribute";
        public static readonly string TEST_ATTR = "Xunit.FactAttribute";
        public static readonly string ROW_ATTR = "Xunit.InlineDataAttribute";
        public static readonly string CATEGORY_ATTR = "Xunit.TraitAttribute";
        // public static readonly string TESTSETUP_ATTR = "NUnit.Framework.SetUpAttribute";
        // public static readonly string TESTFIXTURESETUP_ATTR = "NUnit.Framework.TestFixtureSetUpAttribute";
        // public static readonly string TESTFIXTURETEARDOWN_ATTR = "NUnit.Framework.TestFixtureTearDownAttribute";
        // public static readonly string TESTTEARDOWN_ATTR = "NUnit.Framework.TearDownAttribute";
        public static readonly string IGNORE_ATTR = "Xunit.FactAttribute(Skip=\"Ignored\")";
        public static readonly string DESCRIPTION_ATTR = "Xunit.TraitAttribute";

    }
}