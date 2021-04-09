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
            TestMemoryModel();
            TestSegmentRegisters();
        }

        public static void TestRegistryView()
        {
            RegistersViewTest registryViewTest = new RegistersViewTest();
            registryViewTest.StartAllTests();
        }

        public static void TestIndexRegisters()
        {
            IndexRegistersTest indexRegistersTest = new IndexRegistersTest();
            indexRegistersTest.StartAllTests();
        }

        public static void TestSegmentRegisters()
        {
            SegmentRegistersTest indexRegistersTest = new SegmentRegistersTest();
            indexRegistersTest.StartAllTests();
        }

        public static void TestGeneralRegistryCommand()
        {
            MOVTest registryCommandTest = new MOVTest();
            registryCommandTest.StartAllTests();
        }

        public static void TestGeneralPurposeRegisters()
        {
            GeneralPurposeRegisterTest registerTest = new GeneralPurposeRegisterTest();
            registerTest.StartAllTests();
        }

        public static void TestMemoryModel()
        {
            MemoryModelTest memoryModelTest = new MemoryModelTest();
            memoryModelTest.StartAllTests();
        }

    }
}
