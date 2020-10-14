using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAPI.Service
{
    /// 类IocTest依赖MyDependency
    /// 依赖倒置原则：高层模块不应该依赖于低层模块，二者都应该依赖于抽象。应该依赖于抽象，而不是依赖细节。
    /// 什么是高层模块？这里的使用者IocTest类就称为高层模块。什么是低层模块？被使用者MyDependency类就称为低层模块。当前示例中高层模块就依赖于低层模块。
    /// 那么这样子有什么不好呢？
    　　///1、面向对象语言开发，就是类与类之间进行交互，如果高层直接依赖低层的细节，细节是多变的，那么低层的变化就导致上层的变化；
    　　///2、如果层数多了，低层的修改会直接水波效应传递到最上层，一点细微的改动都会导致整个系统从下往上的修改。
    public class IocTest
    {
        /// <summary>
        /// 类
        /// </summary>
        public void show()
        {
            MyDependency myDependency = new MyDependency();//全是细节
            myDependency.show();
            Console.WriteLine($"this is{this.GetType().FullName}");
        }
    }

    public class MyDependency
    {
        public void show()
        {
            Console.WriteLine($"this is{this.GetType().FullName}");
        }
    }

    /// 按照依赖倒置原则修改如下
    public class IocTest2
    {
        /// <summary>
        /// 类
        /// </summary>
        public void show2()
        {
            IDepenency myDependency = new MyDependency2();//左边抽象右边细节
            myDependency.show2();
            Console.WriteLine($"this is{this.GetType().FullName}");
        }
    }

    public class MyDependency2 : IDepenency
    {
        public void show2()
        {
            Console.WriteLine($"this is{this.GetType().FullName}");
        }
    }
    public interface IDepenency
    {
        void show2();
    }

    ///上面示例经过改造后虽然遵循了“依赖倒置原则”，但是违背了“开放封闭原则”，因为如果有一天想要修改变量myDependency为YourDependency类的实例，则需要修改Test类。
    /// IOC控制反转
    /// 将 _myDependency 的创建过程“反转”给了调用者。
    public class IocTest3
    {
        private readonly IDepenency2 _myDependency;
        public IocTest3(IDepenency2 myDependency)
        {
            this._myDependency = myDependency;
        }

        public void Show3()
        {
            _myDependency.Show3();
            Console.WriteLine($"This is {this.GetType().FullName}");
        }
    }

    public class MyDependency3 : IDepenency2
    {
        public void Show3()
        {
            Console.WriteLine($"This is {this.GetType().FullName}");
        }
    }

    public interface IDepenency2
    {
        void Show3();
    }
}
