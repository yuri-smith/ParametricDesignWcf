using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using ParametricDesignWcfServiceLibrary.Model;

namespace ParametricDesignWcfServiceLibrary.Model.Context
{
    public class DataContextInitializer : CreateDatabaseIfNotExists<ParametricDesignContext>
    //public class DataContextInitializer : DropCreateDatabaseAlways<ParametricDesignContext>
    //public class DataContextInitializer : DropCreateDatabaseIfModelChanges<ParametricDesignContext>
    {
        protected override void Seed(ParametricDesignContext context)
        {
            context.Roles.AddRange(new Role[]
            {
                new Role()
                {
                    Name = "AdminApp",
                    Descr = "Администратор всех данных"
                },
                new Role()
                {
                    Name = "AdminCompany",
                    Descr = "Администратор данных своей организации"
                },
                new Role()
                {
                    Name = "AdminHimSelf",
                    Descr = "Администратор своих данных"
                },
                new Role()
                {
                    Name = "AdminDemo",
                    Descr = "Администратор данных демо-примеров"
                },
                new Role()
                { 
                    Name = "UserCompany",
                    Descr = "Пользователь данных своей организации"
                },
                new Role()
                { 
                    Name = "UserHimSelf",
                    Descr = "Пользователь своих данных"
                },
                new Role()
                {
                    Name = "UserDemo",
                    Descr = "Пользователь данных демо-примеров"
                },
            });
            context.SaveChanges();
            //Country country = new Country
            //{
            //     Name = "Россия",
            //      Regions = new Region[] 
            //      { 
            //          new Region { Name = "Сибирь" }
            //      }  
            //};
            //context.Countries.Add(country);
            //Region region = new Region { Name = "Татарстан" };
            //context.Regions.Add(region);
            //country.Regions = new Region[] { region };
            //City city = new City { Name = "Набережные Челны", Region = region };
            //context.Cities.Add(city);
            ////region.Cities = new City[] { city };
            //Company company = new Company
            //{
            //    Name = "RBC",
            //    FullName = "RosBizinesConsalting",
            //    LegalCity = city,
            //    LegalAddress = new Address { Street = "Сююмбике", House = "89", Office = "60" }
            //};
            //context.Companies.Add(company);

            Country newCountry = new Country { Name = "Россия" };
            context.Countries.Add(newCountry);

            City legalCity = new City
            {
                Name = "Набережные Челны",
                Regions = new Region[]
                {
                    new Region 
                    { 
                        Name = "Татарстан",
                            Countries = new Country[]
                            {
                                newCountry
                            }
                    },
                    new Region 
                    { 
                        Name = "Поволжье",
                            Countries = new Country[]
                            {
                                newCountry
                            }
                    }
                },
                Countries = new Country[]
                {
                    newCountry
                }
            };

            //Currency rur = new Currency
            //{
            //    Code = "RUR",
            //    Name = "Российский рубль",
            //    Curs = 1.0,
            //    DateCurs = DateTime.Today
            //};
            //Currency eur = new Currency
            //{
            //    Code = "EUR",
            //    Name = "Евро",
            //    Curs = 1.0,
            //    DateCurs = DateTime.Today
            //};
            //Currency usd = new Currency
            //{
            //    Code = "USD",
            //    Name = "Доллар США",
            //    Curs = 1.0,
            //    DateCurs = DateTime.Today
            //};

            Account account = new Account
            {
                Login = "QWERTYUI",
                Password = "12345678",
                Roles = new Role[]
                    {
                        context.Roles.Where(r => r.Name == "AdminApp").FirstOrDefault(),
                        context.Roles.Where(r => r.Name == "AdminCompany").FirstOrDefault(),
                    },
                Person = new Person
                {
                    Name = "Кузнецов Юрий",
                    Company = new Company
                    {
                        INN = "111111111111",
                        KPP = "222222222222",
                        Name = "Татпроф",
                        Currency = "RUR",
                        LegalCity = legalCity,
                        LegalAddress = new Address
                        {
                            Street = "Сююмбике",
                            House = "89",
                            Office = "60"
                        },
                        ActualCity = legalCity,
                        ActualAddress = new Address
                        {
                            Street = "Сююмбике",
                            House = "89",
                            Office = "60"
                        }
                    }
                }
            };
            context.Accounts.Add(account);


                  
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                //System.Windows.
            }

                 
        }

    }
}
