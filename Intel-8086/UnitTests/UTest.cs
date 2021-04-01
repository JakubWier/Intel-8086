namespace Tests_Intel_8086
{
    static class UTest
    {
        public static void StartAllTests()
        {
            TestGeneralPurposeRegisters();
            TestIndexRegisters();
            TestGeneralRegistryCommand();
            TestRegistryView();
        }

        public static void TestRegistryView()
        {
            GeneralPurposeRegistryViewTest registryViewTest = new GeneralPurposeRegistryViewTest();
            registryViewTest.StartAllTests();
        }

        public static void TestIndexRegisters()
        {
            IndexRegistersTest indexRegistersTest = new IndexRegistersTest();
            indexRegistersTest.StartAllTests();
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
