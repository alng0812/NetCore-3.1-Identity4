using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace TestPassport
{
    class Program
    {
        //    static void Main(string[] args)
        //    {
        //        try
        //        {
        //            Console.WriteLine("0:获取Token 1：验证token 2：登录 3：注册 4：修改密码 5：修改基本资料 6：值比较");
        //            int Type = int.Parse(Console.ReadLine());
        //            string username = string.Empty;
        //            string password = string.Empty;
        //            string openid = string.Empty;
        //            string access_token = string.Empty;
        //            string appid = string.Empty;
        //            switch (Type)
        //            {
        //                case 1:
        //                    TestIdentity4Async();
        //                    break;
        //                case 2:
        //                    Console.WriteLine("模拟登录选择→ 0:账户密码 1：微信登录 2：QQ登录");
        //                    int loginType = int.Parse(Console.ReadLine());
        //                    switch (loginType)
        //                    {
        //                        case 1:
        //                            Console.WriteLine("请输入openId");
        //                            openid = Console.ReadLine();
        //                            Console.WriteLine("请输入access_token");
        //                            access_token = Console.ReadLine();
        //                            break;
        //                        case 2:
        //                            Console.WriteLine("请输入openId");
        //                            openid = Console.ReadLine();
        //                            Console.WriteLine("请输入access_token");
        //                            access_token = Console.ReadLine();
        //                            Console.WriteLine("请输入appid");
        //                            appid = Console.ReadLine();
        //                            break;
        //                        default:
        //                            Console.WriteLine("请输入账号");
        //                            username = Console.ReadLine();
        //                            Console.WriteLine("请输入密码");
        //                            password = Console.ReadLine();
        //                            break;
        //                    }
        //                    TestIdentity4Login(username, password);
        //                    break;
        //                case 3:
        //                    TestIdentity4Register();
        //                    break;
        //                case 4:
        //                    TestIdentity4EditPwd();
        //                    break;
        //                case 5:
        //                    TestIdentity4EditAcountInfo();
        //                    break;
        //                case 6:
        //                    TestValueEqual();
        //                    break;
        //                default:
        //                    TestGetAccessToken();
        //                    break;
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine("异常：" + e.Message);
        //        }
        //        Console.ReadKey();
        //    }

        //    public class TokenModel
        //    {
        //        public string ClientId { get; set; }
        //        public string ClientSecret { get; set; }
        //        public string Scope { get; set; }
        //    }

        //    /// <summary>
        //    /// 获取token
        //    /// </summary>
        //    private static async void TestGetAccessToken()
        //    {
        //        var client = new HttpClient();
        //        var responseJson = "";
        //        string url = "http://localhost:5001/api/Account/GetAccessToken";
        //        TokenModel tokenmodel = new TokenModel
        //        {
        //            ClientId = "test",
        //            ClientSecret = "20200930",
        //            Scope = "api"
        //        };
        //        //string data = "{\"ClientId\" : \"Client\",\"ClientSecret\" : \"NewareAI\",\"Scope\" : \"api\"}";
        //        HttpContent content = new StringContent(JsonConvert.SerializeObject(tokenmodel));
        //        client.Timeout = new TimeSpan(1, 0, 0, 0, 0);
        //        client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
        //        client.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
        //        client.DefaultRequestHeaders.Add("ContentType", "application/json");
        //        client.DefaultRequestHeaders.Add("Accept", "*/*");
        //        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        //        //await异步等待回应
        //        var response = await client.PostAsync(url, content);
        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            responseJson = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine(responseJson);
        //        }
        //    }

        //    /// <summary>
        //    /// 验证token
        //    /// </summary>
        //    private static async void TestIdentity4Async()
        //    {
        //        //获取到获取token的url
        //        var client = new HttpClient();
        //        var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
        //        if (disco.IsError)
        //        {
        //            Console.WriteLine("获取到获取token的url失败：" + disco.Error);
        //            return;
        //        }
        //        Console.WriteLine("获取token的url为：" + disco.TokenEndpoint);
        //        Console.WriteLine();

        //        //获取token
        //        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //        {
        //            Address = disco.TokenEndpoint,//就是我们postman请求token的地址
        //            ClientId = "Client",//客户端
        //            ClientSecret = "sercet",//秘钥
        //            Scope = "api"//请求的api
        //        });
        //        if (tokenResponse.IsError)
        //        {
        //            Console.WriteLine("获取token失败：" + tokenResponse.Error);
        //            return;
        //        }
        //        Console.WriteLine("获取token的response：");
        //        int index = 0;
        //        foreach (var proc in tokenResponse.GetType().GetProperties())
        //        {
        //            Console.WriteLine($"{++index}.{proc.Name}：{proc.GetValue(tokenResponse)}");
        //        }
        //        Console.WriteLine();

        //        //模拟客户端调用需要身份认证的api
        //        var apiClient = new HttpClient();
        //        //赋值token（携带token访问）
        //        apiClient.SetBearerToken(tokenResponse.AccessToken);
        //        var url = "http://localhost:5001/Identity";
        //        //var url = "http://localhost:5001/api/Account/Get";
        //        //发起请求
        //        var response = await apiClient.GetAsync(url);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            //请求成功
        //            var content = await response.Content.ReadAsStringAsync();
        //            //Console.WriteLine("请求成功，返回结果是：" + content);
        //            Console.WriteLine("请求成功，返回结果是：" + JArray.Parse(content));
        //        }
        //        else
        //        {
        //            //请求失败
        //            Console.WriteLine($"请求失败，状态码为：{(int)response.StatusCode}，描述：{response.StatusCode.ToString()}");
        //        }
        //    }

        //    /// <summary>
        //    /// 登录
        //    /// </summary>
        //    private static async void TestIdentity4Login(string username, string Password)
        //    {
        //        //获取到获取token的url
        //        var client = new HttpClient();
        //        var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
        //        if (disco.IsError)
        //        {
        //            Console.WriteLine("获取到获取token的url失败：" + disco.Error);
        //            return;
        //        }
        //        Console.WriteLine("获取token的url为：" + disco.TokenEndpoint);
        //        Console.WriteLine();

        //        //获取token
        //        var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //        {
        //            Address = disco.TokenEndpoint,//
        //            ClientId = "Client",//客户端
        //            ClientSecret = "sercet",//秘钥
        //            Scope = "api"//请求的api
        //        });
        //        if (tokenResponse.IsError)
        //        {
        //            Console.WriteLine("获取token失败：" + tokenResponse.Error);
        //            return;
        //        }
        //        Console.WriteLine("获取token的response：");
        //        int index = 0;
        //        foreach (var proc in tokenResponse.GetType().GetProperties())
        //        {
        //            Console.WriteLine($"{++index}.{proc.Name}：{proc.GetValue(tokenResponse)}");
        //        }
        //        Console.WriteLine();

        //        //模拟客户端调用需要身份认证的api
        //        var apiClient = new HttpClient();
        //        //赋值token（携带token访问）
        //        apiClient.SetBearerToken(tokenResponse.AccessToken);
        //        var url = string.Format("http://localhost:5001/api/Login/Login?logintype=mobileOrEmail&username={0}&password={1}", username, Password);
        //        //发起请求
        //        var response = await apiClient.GetAsync(url);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            //请求成功
        //            var content = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine("请求成功，返回结果是：" + (JObject)JsonConvert.DeserializeObject(content));
        //        }
        //        else
        //        {
        //            //请求失败
        //            Console.WriteLine($"请求失败，状态码为：{(int)response.StatusCode}，描述：{response.StatusCode.ToString()}");
        //        }
        //    }

        //    /// <summary>
        //    /// 注册
        //    /// </summary>
        //    private static async void TestIdentity4Register()
        //    {

        //    }

        //    /// <summary>
        //    /// 修改密码
        //    /// </summary>
        //    private static async void TestIdentity4EditPwd()
        //    {

        //    }

        //    /// <summary>
        //    /// 修改基本资料
        //    /// </summary>
        //    private static async void TestIdentity4EditAcountInfo()
        //    {

        //    }

        //    private static async void TestValueEqual()
        //    {
        //        int[] list = { 1, 3, 5, 2, 4, 6, 10, 7, 8, 9 };
        //        var result = list.OrderByDescending(o => o).ThenBy(t => t);
        //        string r = string.Join("", result);
        //        Console.WriteLine(r);
        //        var nums = new int[7] { 0, 1, 2, 3, 4, 5, 6 };
        //        var f = nums.Reverse();
        //        Console.WriteLine(string.Join("", f));
        //        List<int> a = new List<int> { 1, 2, 3, 4, 5, 7, 6, 8 };
        //        List<int> b = new List<int> { 9, 10, 11, 12, 13, 14, 15, 16 };
        //        List<int> c = new List<int> { 1, 2, 4, 3, 5, 6, 7, 8 };
        //        var aaa = a.OrderBy(a => a);
        //        var ccc = a.OrderBy(c => c);
        //        string aa = string.Join("", aaa);
        //        string bb = string.Join("", b);
        //        string cc = string.Join("", ccc);
        //        Console.WriteLine(aa.Equals(bb));
        //        Console.WriteLine(aa.Equals(cc));
        //        //Console.WriteLine(a.SequenceEqual(aaa));
        //        Console.WriteLine(aaa.SequenceEqual(ccc));
        //    }
        //}
        //internal class Program
        //{
        //    private static void Main(string[] args)
        //    {
        //        A a = new A();
        //        Console.WriteLine($"1-2={(a.func1(1, 2))}");

        //        B b = new B();
        //        Console.WriteLine($"1-2={(b.func1(1, 2))}");
        //        Console.WriteLine($"1-2={(b.func2(1, 2))}");

        //        Console.ReadKey();
        //    }
        //}

        //internal class A
        //{
        //    public int func1(int num1, int num2)
        //    {
        //        return num1 - num2;
        //    }
        //}

        //internal class B : A
        //{
        //    public int func1(int num1, int num2)
        //    {
        //        return num1 + num2;
        //    }

        //    public int func2(int num1, int num2)
        //    {
        //        return func1(num1, num2) + 1;
        //    }
        //}

        //internal class Program
        //{
        //    private static void Main(string[] args)
        //    {
        //        Agent agent = new Agent();
        //        agent.setCustomer(new Customer("小杨"));
        //        agent.setSeller(new Seller("世纪阳光小区业主 张女士"));
        //        agent.setCompany(new House("NewAve中骏天成阳关楼盘 小燕子"));
        //        agent.viewHouse();
        //        agent.drinkCoffee();
        //    }
        //}

        //internal class Agent
        //{
        //    private Customer customer;
        //    private Seller seller;
        //    private House house;

        //    public void setCustomer(Customer _customer)
        //    {
        //        this.customer = _customer;
        //    }

        //    public void setSeller(Seller _seller)
        //    {
        //        this.seller = _seller;
        //    }

        //    public void setCompany(House _house)
        //    {
        //        this.house = _house;
        //    }

        //    public void viewHouse()
        //    {
        //        Console.WriteLine($"{house.getName()}与客户{customer.getName()}联系看样板房。");
        //    }

        //    public void drinkCoffee()
        //    {
        //        Console.WriteLine($"{seller.getName()}与客户{customer.getName()}相约壹咖啡喝杯下午茶。");
        //    }
        //}

        //internal class Customer
        //{
        //    private string name;

        //    public Customer(string name)
        //    {
        //        this.name = name;
        //    }

        //    public string getName()
        //    {
        //        return this.name;
        //    }
        //}

        //internal class House
        //{
        //    private string name;

        //    public House(string name)
        //    {
        //        this.name = name;
        //    }

        //    public string getName()
        //    {
        //        return this.name;
        //    }
        //}

        //internal class Seller
        //{
        //    private string name;

        //    public Seller(string name)
        //    {
        //        this.name = name;
        //    }

        //    public string getName()
        //    {
        //        return this.name;
        //    }
        //}

        //internal class Program
        //{
        //    private static void Main(string[] args)
        //    {
        //        CommonMethod commonMethod = new CommonMethod();
        //        commonMethod.insert();
        //        commonMethod.countTotal();
        //        commonMethod.printInfo();
        //    }
        //}

        //internal interface InputModule
        //{
        //    void insert();
        //    void delete();
        //    void modify();
        //}

        //internal interface CountModule
        //{
        //    void countTotal();

        //    void countAverage();
        //}

        //internal interface PrintModule
        //{
        //    void printInfo();

        //    void queryInfo();
        //}

        //internal class CommonMethod : InputModule, CountModule, PrintModule
        //{
        //    public void insert()
        //    {
        //        Console.WriteLine("输入模块的insert()方法被调用！");
        //    }
        //    public void delete()
        //    {
        //        Console.WriteLine("输入模块的delete()方法被调用！");
        //    }
        //    public void modify()
        //    {
        //        Console.WriteLine("输入模块的modify()方法被调用！");
        //    }
        //    public void countTotal()
        //    {
        //        Console.WriteLine("统计模块的countTotal()方法被调用！");
        //    }
        //    public void countAverage()
        //    {
        //        Console.WriteLine("统计模块的countAverage()方法被调用！");
        //    }
        //    public void printInfo()
        //    {
        //        Console.WriteLine("打印模块的printInfo()方法被调用！");
        //    }
        //    public void queryInfo()
        //    {
        //        Console.WriteLine("打印模块的queryInfo()方法被调用！");
        //    }
        //}

        public class Test
        {
            public string GetSomething()
            {
                return "Good";
            }
            public string DoSomething(int item)
            {
                return "I Do";
            }
            public int returnSomething(int item)
            {
                return 1;
            }
            public void voidSomething(int item)
            {

            }
        };
        //private static void Main(string[] args)
        //{
        //    //创建实例
        //    Test test = new Test();
        //    //获取类型
        //    Type testType = test.GetType();
        //    //获取类型的方法列表
        //    //BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public 这个有一个注意点
        //    //实际上至少要有Instance（或Static）与Public（或NonPublic）标记。否则将不会获取任何方法。
        //    MethodInfo[] obj = testType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        //    //遍历所有的方法
        //    foreach (MethodInfo item in obj)
        //    {
        //        //返回方法的返回类型
        //        Console.Write(item.ReturnType.Name);
        //        //返回方法的名称
        //        Console.Write(" " + item.Name + "(");
        //        //获取方法的返回参数列表
        //        ParameterInfo[] parameterss = item.GetParameters();
        //        foreach (var parameters in parameterss)
        //        {
        //            //参数类型名称
        //            Console.Write(parameters.ParameterType.Name);
        //            //参数名称
        //            Console.Write(" " + parameters.Name + ",");
        //        }
        //        Console.WriteLine(")");
        //    }
        //}

        private static void Main(string[] args)
        {

            //    //创建实例
            Test test = new Test();
            //获取泛性的Type类型
            Type objType = test.GetType();
            //获取泛性的方法列表
            MethodInfo[] mthodInfos = objType.GetMethods();
            //循环方法
            foreach (var item in mthodInfos)
            {
                //获取方法的所有参数列表
                var parameters = item.GetParameters();
                //过滤没用方法
                //1:查看是不是有参数的方法
                //2:查看这个方法的返回类型是不是我们想要的
                if (parameters.Any() &&
                    parameters[0].ParameterType == typeof(int) &&
                    item.ReturnType != typeof(void))
                {
                    //调用方法
                    object[] parametersObj = new object[] { 5 };
                    //调用实例方法
                    //第一个参数是我们的实体，后面是我们的参数（参数是一个数组，多个参数按照顺序来传递,没有参数可以为null）
                    //如果我们的方法是一个静态方法 ，这个参数可以为null （不是静态的就会报错）
                    Console.WriteLine(item.Invoke(test, parametersObj));
                }
            }
        }
    }
}
