namespace Tests_Intel_8086
{
    static class UTest
    {
        public static void StartAllTests()
        {
            TestGeneralPurposeRegisters();
            TestGeneralRegistryCommand();
            TestRegistryView();
        }

        public static void TestRegistryView()
        {
            RegistryViewTest registryViewTest = new RegistryViewTest();
            registryViewTest.StartAllTests();
        }

        public static void TestGeneralRegistryCommand()
        {
            GeneralPurposeRegistryCommandTest registryCommandTest = new GeneralPurposeRegistryCommandTest();
            registryCommandTest.StartAllTests();
        }

        public static void TestGeneralPurposeRegisters()
        {
            GeneralPurposeRegisterTest registerTest = new GeneralPurposeRegisterTest();
            registerTest.StartAllTests();
        }

    }
}
