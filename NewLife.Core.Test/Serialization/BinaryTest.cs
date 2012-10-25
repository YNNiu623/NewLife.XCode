﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NewLife.Serialization;
using System.Text;

namespace NewLife.Core.Test.Serialization
{
    /// <summary>
    /// BinaryWriterTest 的摘要说明
    /// </summary>
    [TestClass]
    public class BinaryTest
    {
        public BinaryTest()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        void TestWriter(Obj obj)
        {
            var set = new BinarySettings();
            for (int i = 0; i < 36; i++)
            {
                set.UseObjRef = (i & 0x01) == 1;
                set.UseTypeFullName = (i >> 1 & 0x01) == 1;
                set.Encoding = (i >> 2 & 0x01) == 1 ? Encoding.Default : Encoding.UTF8;
                set.EncodeInt = (i >> 3 & 0x01) == 1;
                set.SizeFormat = (i >> 4 & 0x01) == 1 ? TypeCode.Int32 : TypeCode.UInt32;
                set.SplitComplexType = (i >> 5 & 0x01) == 1;

                TestWriter(obj, set);
            }
        }

        void TestWriter(Obj obj, BinarySettings set)
        {
            try
            {
                // 二进制序列化写入
                var writer = new BinaryWriterX();
                writer.Settings = set;
                writer.WriteObject(obj);

                var bts1 = writer.Stream.ReadBytes();

                // 对象本应有的数据流
                var bts2 = obj.GetStream(writer.Settings).ReadBytes();

                var n = bts1.CompareTo(bts2);
                Assert.AreEqual(0, n, "二进制写入器得到的数据与标准不符！");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + " " + ex.TargetSite);
            }
        }

        [TestMethod]
        public void TestWriter()
        {
            var obj = SimpleObj.Create();
            TestWriter(obj);
        }

        [TestMethod]
        public void TestWriteArray()
        {
            var obj = new ArrayObj();
            TestWriter(obj);

            obj.Objs = null;
            TestWriter(obj);
        }

        [TestMethod]
        public void TestWriteList()
        {
            var obj = new ListObj();
            TestWriter(obj);

            obj.Objs = null;
            TestWriter(obj);
        }

        [TestMethod]
        public void TestWriteDictionary()
        {
            var obj = new DictionaryObj();
            TestWriter(obj);

            obj.Objs = null;
            TestWriter(obj);
        }

        [TestMethod]
        public void TestWriteExtend()
        {
            try
            {
                var obj = new ExtendObj();
                TestWriter(obj);

                obj = ExtendObj.Create();
                for (int i = 0; i < 100; i++)
                {
                    TestWriter(obj);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + " " + ex.TargetSite);
            }
        }

        void TestReader(Obj obj)
        {
            var set = new BinarySettings();
            for (int i = 0; i < 36; i++)
            {
                set.UseObjRef = (i & 0x01) == 1;
                set.UseTypeFullName = (i >> 1 & 0x01) == 1;
                set.Encoding = (i >> 2 & 0x01) == 1 ? Encoding.Default : Encoding.UTF8;
                set.EncodeInt = (i >> 3 & 0x01) == 1;
                set.SizeFormat = (i >> 4 & 0x01) == 1 ? TypeCode.Int32 : TypeCode.UInt32;
                set.SplitComplexType = (i >> 5 & 0x01) == 1;

                TestReader(obj, set);
            }
        }

        void TestReader(Obj obj, BinarySettings set)
        {
            try
            {
                var reader = new BinaryReaderX();
                reader.Settings = set;

                // 获取对象的数据流，作为二进制读取器的数据源
                reader.Stream = obj.GetStream(reader.Settings);
                // 读取一个跟原始对象类型一致的对象
                var obj2 = reader.ReadObject(obj.GetType());

                Assert.IsNotNull(obj2, "二进制读取器无法读取标准数据！");

                var b = obj.CompareTo(obj2 as Obj);
                Assert.IsTrue(b, "序列化后对象不一致！");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + " " + ex.TargetSite);
            }
        }

        [TestMethod]
        public void TestReader()
        {
            var obj = SimpleObj.Create();
            TestReader(obj);
        }

        [TestMethod]
        public void TestReadArray()
        {
            var obj = new ArrayObj();
            TestReader(obj);

            obj.Objs = null;
            TestReader(obj);
        }

        [TestMethod]
        public void TestReadList()
        {
            var obj = new ListObj();
            TestReader(obj);

            obj.Objs = null;
            TestReader(obj);
        }

        [TestMethod]
        public void TestReadDictionary()
        {
            var obj = new DictionaryObj();
            TestReader(obj);

            obj.Objs = null;
            TestReader(obj);
        }

        [TestMethod]
        public void TestReadExtend()
        {
            try
            {
                var obj = new ExtendObj();
                TestReader(obj);

                obj = ExtendObj.Create();
                for (int i = 0; i < 100; i++)
                {
                    TestReader(obj);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message + " " + ex.TargetSite);
            }
        }
    }
}