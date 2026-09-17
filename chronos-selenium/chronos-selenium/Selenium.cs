using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace chronos_selenium
{
    public class Selenium
    {
        IWebDriver driver;
        public Selenium()
        {
            ChromeOptions options = new ChromeOptions();

            options.AddUserProfilePreference(
                "profile.password_manager_leak_detection",
                false
            );

            driver = new ChromeDriver(options);

            driver.Manage().Window.Position = new System.Drawing.Point(-1440, 0);
        }

        public void iniciar ()
        {
            driver.Navigate().GoToUrl("http://localhost:4200/");
            driver.Manage().Window.Maximize();
            
            var js = (IJavaScriptExecutor)driver;

           
            bool continuar = true;

            while (continuar)
            {

              string valor = (string)js.ExecuteScript(
                "return localStorage.getItem('token');");

                if (valor == null)
                {

                    IWebElement matricula = driver.FindElement(By.Id("registration"));
                    IWebElement senha = driver.FindElement(By.Id("password"));

                    string valorMatricula = matricula.GetAttribute("value");
                    string valorSenha = senha.GetAttribute("value");

                    if (string.IsNullOrEmpty(valorMatricula))
                    {
                        matricula.SendKeys("123456789");
                    }

                    if (string.IsNullOrEmpty(valorSenha))
                    {
                        senha.SendKeys("123456789");
                    }

                    IWebElement logar = driver.FindElement(By.Id("login"));

                    logar.Click();
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
                else
                {
                    driver.Navigate().GoToUrl("http://localhost:4200/manager");

                    IWebElement junior = driver.FindElement(By.Id("junior"));

                    junior.Click();
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    IWebElement pleno = driver.FindElement(By.Id("pleno"));

                    pleno.Click();
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    IWebElement senior = driver.FindElement(By.Id("senior"));

                    senior.Click();
                    Thread.Sleep(TimeSpan.FromSeconds(5));

                    driver.Navigate().GoToUrl("http://localhost:4200/reports");

                    //IWebElement main = driver.FindElement(By.ClassName("page-layout"));

                    //WheelInputDevice.ScrollOrigin scrollOrigin = new WheelInputDevice.ScrollOrigin
                    //{
                    //    Element = main
                    //};

                    //new Actions(driver).ScrollFromOrigin(scrollOrigin, 0, 200).Perform();

                    Thread.Sleep(TimeSpan.FromSeconds(20));
                }
            }
        }
    }
}
