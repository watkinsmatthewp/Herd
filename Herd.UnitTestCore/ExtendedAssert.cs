using Herd.Core;
using Xunit;

namespace Herd.UnitTestCore
{
    public static class ExtendedAssert
    {
        /// <summary>
        /// Asserts that the objects serialize to the same values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poco1"></param>
        /// <param name="poco2"></param>
        public static void ObjectsEqual<T>(T poco1, T poco2) where T : class
        {
            Assert.Equal(poco1 == null, poco2 == null);
            if (poco1 != null)
            {
                Assert.Equal(poco1.SerializeAsJson(), poco2.SerializeAsJson());
            }
        }
    }
}