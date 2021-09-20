using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ParametricDesignWcfServiceLibrary.Model;
using ParametricDesignWcfServiceLibrary.Model.Context;
//using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.IO;
//using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace ParametricDesignWcfServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public Guid GetSession(string UserName, string Password, bool WithRegistration = false)
        {
            //Validator
            //https://msdn.microsoft.com/ru-ru/library/aa702565(v=vs.110).aspx
            //PrincipalPermission permission = new PrincipalPermission(
            Guid newGuid = Guid.Empty;
            using (ParametricDesignContext dbContext = new ParametricDesignContext())
            {
                dbContext.Database.CommandTimeout = 1800;
                //Account account = null;
                Account account = dbContext.Accounts.Include("Roles").Where(a => a.Login == UserName).FirstOrDefault();
                if (WithRegistration)
                {
                    if (account != null)
                    {
                        CustomExpMsg customMsg = new CustomExpMsg("Пользователь с таким логином уже существует!", 1);
                        throw new FaultException<CustomExpMsg>(customMsg,
                            new FaultReason(customMsg.ErrorMsg));
                    }
                    dbContext.Accounts.Add(new Account()
                    {
                        Login = UserName,
                        Password = Password,
                        Roles = new Role[] { dbContext.Roles.Find(3), 
                                                    dbContext.Roles.Find(4) }
                    });
                    try
                    {
                        dbContext.SaveChanges();
                        CustomExpMsg customMsg = new CustomExpMsg("Пользователь зарегистрирован как Администратор только своих данных!", 2);
                        throw new FaultException<CustomExpMsg>(customMsg,
                            new FaultReason(customMsg.ErrorMsg));
                    }
                    catch (DbEntityValidationException ex1)
                    {
                        var errorMessages = ex1.EntityValidationErrors
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.ErrorMessage);
                        var fullErrorMessage = string.Join(";\n", errorMessages);

                        CustomExpMsg customMsg = new CustomExpMsg(fullErrorMessage, 1);
                        throw new FaultException<CustomExpMsg>(customMsg,
                            new FaultReason(customMsg.ErrorMsg));
                    }
                }
                else
                {
                    if (account == null)
                    {
                        CustomExpMsg customMsg = new CustomExpMsg("Нет такого пользователя!", 2);
                        throw new FaultException<CustomExpMsg>(customMsg,
                            new FaultReason(customMsg.ErrorMsg));
                    }
                    if (account.Password != Password)
                    {
                        CustomExpMsg customMsg = new CustomExpMsg("Неверный пароль!", 2);
                        throw new FaultException<CustomExpMsg>(customMsg,
                            new FaultReason(customMsg.ErrorMsg));
                    }
                    if (!account.Roles.Contains(dbContext.Roles.Find(1)) &&
                        !account.Roles.Contains(dbContext.Roles.Find(2)) &&
                        !account.Roles.Contains(dbContext.Roles.Find(3)))
                    {
                        CustomExpMsg customMsg = new CustomExpMsg("Недостаточно прав!", 3);
                        throw new FaultException<CustomExpMsg>(customMsg,
                            new FaultReason(customMsg.ErrorMsg));
                    }
                    Session sessionOfAccount = dbContext.Sessions.
                        Where(s => s.AccountAccountID == account.AccountID &&
                            s.DateClose == null).FirstOrDefault();
                    if (sessionOfAccount != null)
                    {
                        CustomExpMsg customMsg = new CustomExpMsg("Пользователь уже работает!", 4);
                        throw new FaultException<CustomExpMsg>(customMsg,
                            new FaultReason(customMsg.ErrorMsg));
                    }
                    newGuid = Guid.NewGuid();
                    dbContext.Sessions.Add(new Session()
                    {
                        Account = account,
                        DateCreate = DateTime.Now,
                        SessionID = newGuid
                    });
                    dbContext.SaveChanges();
                }
            }
            return newGuid;
        }

        public void CloseSession(Guid SessionGuid)
        {
            using (ParametricDesignContext dbContext = new ParametricDesignContext())
            {
                dbContext.Sessions.Where(s => s.SessionID == SessionGuid).FirstOrDefault<Session>().DateClose = DateTime.Now;
                //dbContext.Sessions.Remove(dbContext.Sessions.Where(s => s.SessionID == SessionGuid).FirstOrDefault<Session>());
                dbContext.SaveChanges();
            }
        }

        //public int AddCompany(string Name)
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        Company newCompany = new Company { Name = Name };
        //        context.Companies.Add(newCompany);
        //        context.SaveChanges();
        //        return newCompany.CompanyID;
        //    }

        //}

        //public int AddCustomer(int SellerID, string Name)
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        int newCustomerID = AddCompany(Name);
        //        context.SellerCustomerCompanies.Add(new SellerCustomerCompany
        //        {
        //            SellerCompanyID = SellerID,
        //            CustomerCompanyID = newCustomerID,
        //            Discont = 10
        //        });
        //        context.SaveChanges();
        //        return newCustomerID;
        //    }
        //}

        public List<Company> GetCustomers(int SellerID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Company> cs = new List<Company>();
                //Company seller = context.Companies.Include(x => x.Sellers).Include(x => x.Customers).Where(c => c.CompanyID == SellerID).FirstOrDefault<Company>();
                //Company cmp = context.Companies.Include("Persons").Where(c => c.CompanyID == Id).FirstOrDefault<Company>();
                int[] ids = context.SellerCustomerCompanies.Where(s => s.SellerCompanyID == SellerID)
                    .Select(x => x.CustomerCompanyID).ToArray();
                var ccc = context.Companies.Where(c => ids.Contains(c.CompanyID));
                foreach (Company c in ccc)
                {
                    cs.Add(new Company
                    {
                        CompanyID = c.CompanyID,
                        Name = c.Name
                    });
                }
                //var cc = from cst in context.Companies.Include("Sellers").Include("Customers")
                //             where cst.CompanyID == SellerID
                //             select cst;
                //var ccc = from ll in cc select ll.s 
                //foreach (SellerCustomerCompany scc in c.Customers)
                //{
                //    cs.Add(scc.Customer);
                //}
                //return c.Customers.Select(x => x.Customer).ToList();
                return cs;
            }
        }


        public string GetCompanyName(int Id)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                return context.Companies.Find(Id).Name;
            }
        }

        public Company GetCompany(int Id)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.Database.CommandTimeout = 1800;
                Company cmp = context.Companies.Find(Id);
                Company cmp1 = new Company()
                {
                    CompanyID = Convert.ToInt32(cmp.CompanyID.ToString()),
                    INN = cmp.INN,
                    KPP = cmp.KPP,
                    Name = cmp.Name,
                    LongName = cmp.LongName,
                    LegalCityCityID = Convert.ToInt32(cmp.LegalCityCityID.ToString()),
                    //LegalAddress = new Address()
                    //{
                    //    Street = cmp.LegalAddress.Street,
                    //    House = cmp.LegalAddress.House,
                    //    Office = cmp.LegalAddress.Office
                    //},
                    ActualCityCityID = Convert.ToInt32(cmp.ActualCityCityID.ToString()),
                    //ActualAddress = new Address
                    //{
                    //    Street = cmp.ActualAddress.Street,
                    //    House = cmp.ActualAddress.House,
                    //    Office = cmp.ActualAddress.Office
                    //}
                };
                cmp1.LegalAddress.Street = cmp.LegalAddress.Street;
                cmp1.LegalAddress.House = cmp.LegalAddress.House;
                cmp1.LegalAddress.Office = cmp.LegalAddress.Office;
                cmp1.ActualAddress.Street = cmp.ActualAddress.Street;
                cmp1.ActualAddress.House = cmp.ActualAddress.House;
                cmp1.ActualAddress.Office = cmp.ActualAddress.Office;
                //Company cmp = context.Companies.Include("Persons").Where(c => c.CompanyID == Id).FirstOrDefault<Company>();
                //List<Person> list = new List<Person>();
                //foreach (Person prs in cmp.Persons)
                //{
                //    list.Add(new Person
                //    {
                //        PersonID = Convert.ToInt32(prs.PersonID.ToString()),
                //        Name = prs.Name
                //    });
                //}
                //Company cmp1 = new Company
                //{
                //    CompanyID = Convert.ToInt32(cmp.CompanyID.ToString()),
                //    Name = cmp.Name,
                //    Persons = list.ToArray<Person>()
                //};
                return cmp1;
            }
        }

        public List<Company> GetAllCompanies()
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Company> list = new List<Company>();
                var cs = context.Companies;
                foreach (Company c in cs)
                {
                    list.Add(new Company
                    {
                        CompanyID = Convert.ToInt32(c.CompanyID.ToString()),
                        Name = c.Name
                    });
                }
                return list;
            }
        }

        public List<Person> GetPersonsOfCompany(int Id)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Person> list = new List<Person>();
                var ps = context.Persons.Where(p => p.CompanyCompanyID == Id);
                foreach (Person pr in ps)
                {
                    list.Add(new Person
                    {
                        PersonID = Convert.ToInt32(pr.PersonID.ToString()),
                        Name = pr.Name
                    });
                }
                return list;
            }
        }

        public void AddPersonToCompany(int Id, string Name)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.Persons.Add(new Person
                {
                    CompanyCompanyID = Id,
                    Name = Name
                });
                context.SaveChanges();
            }
        }

        public int AddType(Guid Session, int? ParentTypeID,
            string NameNewTypeProduct)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                TypeProduct newType = new TypeProduct
                {
                    Name = NameNewTypeProduct,
                    AccountAccountID = GetAccountID(Session)
                };
                if (ParentTypeID != null)
                {
                    var pp = context.TypeProductParameters
                        .Where(c => c.TypeProductTypeProductID == ParentTypeID);

                    TypeProduct parent = context.TypeProducts.Find(ParentTypeID);
                    if (parent.ChildTypeProducts == null)
                        parent.ChildTypeProducts = new List<TypeProduct>();
                    newType.TypeProductParameters = new List<TypeProductParameter>();
                    foreach (TypeProductParameter tpp in pp)
                    {
                        newType.TypeProductParameters.Add(new TypeProductParameter
                        {
                            ParameterParameterID = tpp.ParameterParameterID,
                            DefaultValue = tpp.DefaultValue
                        });
                    }
                    parent.ChildTypeProducts.Add(newType);
                }
                else
                {
                    context.TypeProducts.Add(newType);
                }
                context.SaveChanges();
                return newType.TypeProductID;
            }
        }

        public void RenameType(Guid Session, int TypeID, string NewName)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.TypeProducts.Find(TypeID).Name = NewName;
                try
                {
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex1)
                {
                    var errorMessages = ex1.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);
                    var fullErrorMessage = string.Join(";\n", errorMessages);

                    CustomExpMsg customMsg = new CustomExpMsg(fullErrorMessage, 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }

            }
        }
        public List<TypeProductParameter> GetTypeParameters(Guid Session, int TypeID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<TypeProductParameter> list = new List<TypeProductParameter>();
                var tpps = context.TypeProductParameters.Include("Parameter")
                    .Where(pp => pp.TypeProductTypeProductID == TypeID);
                foreach (var tpp in tpps)
                {
                    list.Add(new TypeProductParameter
                    {
                        TypeProductTypeProductID = tpp.TypeProductTypeProductID,
                        ParameterParameterID = tpp.ParameterParameterID,
                        DefaultValue = tpp.DefaultValue,
                        Parameter = new Parameter
                        {
                             ParameterID = tpp.Parameter.ParameterID,
                             Name = tpp.Parameter.Name
                        },
                        //ParameterName = tpp.Parameter.Name
                    });
                };
                return list;
                //return context.TypeProductParameters
                //    .Where(pp => pp.TypeProductTypeProductID == TypeID).ToList<TypeProductParameter>();
                //return new List<TypeProductParameter>(context.TypeProductParameters
                //    .Where(pp => pp.TypeProductTypeProductID == TypeID));
            }
        }

        public List<TypeProductParameter> GetTypeProductParametersFromCombinationID(int CombinationID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<TypeProductParameter> list = new List<TypeProductParameter>();
                int typeProductID = context.Combinations.Find(CombinationID).TypeProductTypeProductID;
                List<TypeProductParameter> typeProductParameters = context.TypeProductParameters
                    .Include("Parameter")
                    .Where(tpp => tpp.TypeProductTypeProductID == typeProductID)
                    .OrderBy(tpp => tpp.Parameter.Name).ToList();
                foreach (TypeProductParameter tpp in typeProductParameters)
                {
                    list.Add(new TypeProductParameter
                    {
                        TypeProductTypeProductID = tpp.TypeProductTypeProductID,
                        ParameterParameterID = tpp.ParameterParameterID,
                        DefaultValue = tpp.DefaultValue,
                        ParameterName = tpp.Parameter.Name
                    });
                }

                return list;
            }
        }

        public List<TypeProduct> GetTypes(Guid Session, int? ParentTypeID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                //if(ParentTypeID == null)
                //List<TypeProduct> list = context.TypeProducts.Where(t => t.ParentTypeProductID == null).ToList<TypeProduct>();
                //return list;
                List<TypeProduct> list = new List<TypeProduct>();
                int accountID = GetAccountID(Session);
                var tps = context.TypeProducts.Where(t => t.ParentTypeProductID == ParentTypeID
                    && t.AccountAccountID == accountID);
                foreach (TypeProduct tp in tps)
                {
                    list.Add(new TypeProduct
                    {
                        TypeProductID = tp.TypeProductID,
                        Name = tp.Name,
                        AccountAccountID = tp.AccountAccountID,
                        ParentTypeProductID = tp.ParentTypeProductID
                    });
                }
                return list;
            }
        }

        public TypeProduct GetType(int TypeID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                //TypeProduct tpr = context.TypeProducts.Include("TypeProductParameters")
                //    .Where(tp => tp.TypeProductID == TypeID).FirstOrDefault<TypeProduct>();
                TypeProduct tpr = context.TypeProducts.Where(tp => tp.TypeProductID == TypeID).FirstOrDefault();
                //tpr.TypeProductID = Convert.ToInt32(tpr.TypeProductID.ToString());
                //return new TypeProduct
                //    {
                //         TypeProductID = Convert.ToInt32(tpr.TypeProductID.ToString()),
                //         Name = tpr.Name,
                //         AccountAccountID = Convert.ToInt32(tpr.AccountAccountID.ToString())
                //    };
                return new TypeProduct
                {
                    TypeProductID = tpr.TypeProductID,
                    Name = tpr.Name,
                    AccountAccountID = tpr.AccountAccountID
                };
            //return ctx.Accounts.
            //    Include("Person").
            //    SingleOrDefault(a => a.Username == username).ToList();
                //return context.TypeProducts.Where(tp => tp.TypeProductID == TypeID).SingleOrDefault();
            }
        }

        public void DelType(Guid Session, int TypeID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.TypeProducts.Remove(context.TypeProducts.Find(TypeID));
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Есть входящие типы!", 4);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
            }
        }

        public void DelCombination(Guid Session, int CombinationID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.Combinations.Remove(context.Combinations.Find(CombinationID));
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Есть входящие объекты!", 4);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
            }
        }

        int GetAccountID(Guid Session)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                return context.Sessions.Where(s => s.SessionID == Session)
                    .FirstOrDefault<Session>().AccountAccountID;
            }
        }

        public List<Parameter> GetParameters()
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                //return context.Parameters
                //    .Where(p => p.AccountAccountID == GetAccountID(Session)).ToList<Parameter>();
                List<Parameter> list = new List<Parameter>();
                //List<Parameter> list = context.Parameters.ToList<Parameter>();
                var prs = from p in context.Parameters select p;
                foreach (var pr in prs)
                {
                    list.Add(new Parameter
                    {
                         ParameterID = pr.ParameterID,
                         Name = pr.Name
                    });
                }
                return list;
            }
        }


        public int AddParameter(Parameter NewParameter)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Parameters.Any(p => p.Name == NewParameter.Name))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Параметр '" +
                        NewParameter.Name + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                context.Parameters.Add(NewParameter);
                context.SaveChanges();
                return NewParameter.ParameterID;
            }
        }

        public void RenameParameter(Parameter CurParameter)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Parameters.Find(CurParameter.ParameterID).Name != CurParameter.Name)
                {
                    if (context.Parameters.Any(p => p.Name == CurParameter.Name))
                    {
                        //if(context.Parameters.Find(CurParameter.ParameterID).Name == CurParameter
                        CustomExpMsg customMsg = new CustomExpMsg("Параметр '" +
                            CurParameter.Name + "' уже существует!", 1);
                        throw new FaultException<CustomExpMsg>(customMsg,
                            new FaultReason(customMsg.ErrorMsg));
                    }
                    context.Parameters.Find(CurParameter.ParameterID).Name = CurParameter.Name;
                    context.SaveChanges();
                }
            }
        }

        public TypeProductParameter AddTypeParameter(int TypeID, int ParamID, int NewDefaultValue)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                TypeProductParameter tpp = new TypeProductParameter
                {
                    TypeProductTypeProductID = TypeID,
                    ParameterParameterID = ParamID,
                    DefaultValue = NewDefaultValue
                };
                context.TypeProductParameters.Add(tpp);
                context.SaveChanges();
                return tpp;
            }

        }

        public List<Combination> GetCombinations(Guid Session, int TypeID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Combination> list = new List<Combination>();
                var cs = context.Combinations
                    .Where(c => c.TypeProductTypeProductID == TypeID);
                foreach (var c in cs)
                {
                    list.Add(new Combination
                    {
                        TypeProductTypeProductID = c.TypeProductTypeProductID,
                        CombinationID = c.CombinationID,
                        Name = c.Name
                    });
                };
                return list;
            }
        }

        public List<CombinationParameter> GetCombinationParameters(int CombinationID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<CombinationParameter> list = new List<CombinationParameter>();
                var cps = context.CombinationParameters
                    .Where(cp => cp.CombinationCombinationID == CombinationID);
                foreach (var cp in cps)
                {
                    list.Add(new CombinationParameter
                    {
                        CombinationCombinationID = cp.CombinationCombinationID,
                        ParameterParameterID = cp.ParameterParameterID,
                        MinValue = cp.MinValue,
                        MaxValue = cp.MaxValue
                    });
                }
                return list;
            }
        }

        public Combination AddCombination(int TypeID, string NewName)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Combinations.Any(c => c.TypeProductTypeProductID == TypeID & c.Name == NewName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Типоразмер '" +
                        NewName + "' уже существует в данном изделии!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                Combination cmb = new Combination
                {
                    TypeProductTypeProductID = TypeID,
                    Name = NewName,
                    CombinationParameters = new List<CombinationParameter>()
                };

                var ppp = context.TypeProductParameters
                    .Where(c => c.TypeProductTypeProductID == TypeID)
                    .Select(x => x.ParameterParameterID);
                foreach (int prmID in ppp)
                {
                    cmb.CombinationParameters.Add(new CombinationParameter
                    {
                        ParameterParameterID = prmID,
                        MinValue = 0,
                        MaxValue = 0
                    });
                }
                context.Combinations.Add(cmb);
                context.SaveChanges();
                return cmb;
            }
        }

        public void RenameCombination(int TypeID, int CombinationID, string NewName)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Combinations.Any(c => c.TypeProductTypeProductID == TypeID & c.Name == NewName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Типоразмер '" +
                        NewName + "' уже существует в данном изделии!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                context.Combinations.Find(CombinationID).Name = NewName;
                context.SaveChanges();

            }
        }

        public List<Dim> GetDims()
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Dim> list = new List<Dim>();
                var ds = context.Dims;
                foreach (var d in ds)
                {
                    list.Add(new Dim
                    {
                        DimID = d.DimID,
                        Name = d.Name
                    });
                };
                return list;
            }
        }

        public List<CombinationFitting> GetCombinationFittings(int CombinationID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<CombinationFitting> list = new List<CombinationFitting>();
                var cfs = context.CombinationFittings.Include("Fitting")
                    .Where(cf => cf.CombinationCombinationID == CombinationID);
                foreach (var c in cfs)
                {
                    list.Add(new CombinationFitting
                    {
                        CombinationCombinationID = c.CombinationCombinationID,
                        FittingFittingID = c.FittingFittingID,
                        Name = c.Name,
                        Size = c.Size,
                        DimSizeDimID = c.DimSizeDimID,
                        Count = c.Count,
                        DimCountDimID = c.DimCountDimID,
                        FittingArticle = c.Fitting.Article
                    });
                }
                return list;
            }
        }

        public CombinationFitting AddCombinationFitting(int CombinationID, int FittingID,
                                                string Name, int DimCountID, string Count,
                                                int DimSizeID, string Size)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                CombinationFitting newObj = new CombinationFitting
                    {
                        CombinationCombinationID = CombinationID,
                        FittingFittingID = FittingID,
                        Name = Name,
                        DimCountDimID = DimCountID,
                        Count = Count
                        //FittingArticle = context.Fittings.Find(FittingID).Article
                    };
                if(DimSizeID > 0)
                {
                    newObj.DimSizeDimID = DimSizeID;
                    newObj.Size = Size;
                }
                context.CombinationFittings.Add(newObj);
                context.SaveChanges();
                //newObj.Fitting = context.Fittings.Find(FittingID);
                newObj.FittingArticle = context.Fittings.Find(FittingID).Article;
                return new CombinationFitting
                    {
                        CombinationCombinationID = newObj.CombinationCombinationID,
                        FittingFittingID = newObj.FittingFittingID,
                        Name = newObj.Name,
                        DimCountDimID = newObj.DimCountDimID,
                        Count = newObj.Count,
                        DimSizeDimID = newObj.DimSizeDimID,
                        Size = newObj.Size,
                        FittingArticle = newObj.FittingArticle
                    };
            }
        }

        public int AddCombinationFitting_1(CombinationFitting CombinationFitting)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.CombinationFittings.Add(CombinationFitting);
                context.SaveChanges();
                return CombinationFitting.CombinationFittingID;
            }

        }

        public void EditCombinationFitting(CombinationFitting CombinationFitting)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                CombinationFitting combinationFitting = context.CombinationFittings
                    .Find(CombinationFitting.CombinationFittingID);
                combinationFitting.Name = CombinationFitting.Name;
                combinationFitting.Count = CombinationFitting.Count;
                combinationFitting.DimCountDimID = CombinationFitting.DimCountDimID;
                combinationFitting.Size = CombinationFitting.Size;
                combinationFitting.DimSizeDimID = CombinationFitting.DimSizeDimID;
                context.SaveChanges();
            }
        }

        public void DelCombinationFitting(int CombinationFittingID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.CombinationFittings.Remove(context.CombinationFittings.Find(CombinationFittingID));
                context.SaveChanges();
            }
        }

        //public List<Dim> GetDims()
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        return context.Dims.ToList<Dim>();
        //    }
        //}

        public string GetArticle(int FittingID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                return context.Fittings.Find(FittingID).Article;
            }
        }

        //public List<Fitting> GetFittings(int SessionID)
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        List<Fitting> list = new List<Fitting>();
        //        int accountID = context.Sessions.Find(SessionID).AccountAccountID;
        //        int? personID = context.Accounts.Find(accountID).PersonPersonID;
        //        int companyID = context.Persons.Find(personID).CompanyCompanyID;
        //        var sellrs = from scs in context.SellerCustomerCompanies
        //                     where scs.CustomerCompanyID == companyID
        //                     select scs.SellerCompanyID;
        //        foreach (int id in sellrs)
        //        {
        //            list.AddRange((from fs in context.Fittings
        //                           where fs.CompanyCompanyID == id
        //                           select fs).AsEnumerable<Fitting>());
        //        }

        //        return list;
        //    }
        //}

        public Dim AddDim(string NewDimName)
        {
            string newName = NewDimName.Trim();
            if (newName.Length == 0)
            {
                CustomExpMsg customMsg = new CustomExpMsg("Не может быть пустой строкой!", 1);
                throw new FaultException<CustomExpMsg>(customMsg,
                    new FaultReason(customMsg.ErrorMsg));
            }
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Dims.Any(d => d.Name == newName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Единица измерения '" +
                        newName + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                Dim newDim = new Dim
                {
                    Name = newName
                };
                context.Dims.Add(newDim);
                context.SaveChanges();
                return newDim;
            }
        }

        public void RenameDim(int DimID, string NewName)
        {
            string newName = NewName.Trim();
            if (newName.Length == 0)
            {
                CustomExpMsg customMsg = new CustomExpMsg("Не может быть пустой строкой!", 1);
                throw new FaultException<CustomExpMsg>(customMsg,
                    new FaultReason(customMsg.ErrorMsg));
            }
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Dims.Any(d => d.Name == newName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Единица измерения '" +
                        newName + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                context.Dims.Find(DimID).Name = newName;
                context.SaveChanges();
            }
        }

        public void DelDim(int DimID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                Dim dim = context.Dims.Find(DimID);
                context.Dims.Remove(dim);
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    CustomExpMsg customMsg = new CustomExpMsg("'" + dim.Name + "' используются!", 4);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
            }
        }

        public List<Country> GetCountries()
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Country> list = new List<Country>();
                //List<Country> cs = context.Countries.Include("Regions").Include("Cities").OrderBy(c => c.Name).ToList();
                var cs = context.Countries.Include("Regions").Include("Cities");
                foreach (Country c in cs)
                {
                    Country cn = new Country
                    {
                        CountryID = c.CountryID,
                        Name = c.Name,
                        //Regions = new List<Region>(),
                        //Cities = new List<City>()
                    };
                    //foreach (Region rg in c.Regions)
                    //{
                    //    cn.Regions.Add(rg);
                    //};
                    //foreach(City ct in c.Cities)
                    //{
                    //    cn.Cities.Add(ct);
                    //};
                    list.Add(cn);
                }
                return list;
            }
        }

        public Country AddCountry(string NewCountryName)
        {
            string newName = NewCountryName.Trim();
            if (newName.Length == 0)
            {
                CustomExpMsg customMsg = new CustomExpMsg("Не может быть пустой строкой!", 1);
                throw new FaultException<CustomExpMsg>(customMsg,
                    new FaultReason(customMsg.ErrorMsg));
            }
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Countries.Any(c => c.Name == newName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Страна '" +
                        newName + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                Country newCountry = new Country
                {
                    Name = newName,
                    Regions = new List<Region>(),
                    Cities = new List<City>()
                };
                context.Countries.Add(newCountry);
                context.SaveChanges();
                return newCountry;
            }
        }

        public void RenameCountry(int CountryID, string NewName)
        {
            string newName = NewName.Trim();
            if (newName.Length == 0)
            {
                CustomExpMsg customMsg = new CustomExpMsg("Не может быть пустой строкой!", 1);
                throw new FaultException<CustomExpMsg>(customMsg,
                    new FaultReason(customMsg.ErrorMsg));
            }
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Countries.Any(c => c.Name == newName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Страна '" +
                        newName + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                context.Countries.Find(CountryID).Name = newName;
                context.SaveChanges();
            }
        }

        public void DelCountry(int CountryID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                Country country = context.Countries.Find(CountryID);
                context.Countries.Remove(country);
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    CustomExpMsg customMsg = new CustomExpMsg("'" + country.Name + "' используется!", 4);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
            }
        }

        //public List<City> GetSities()
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        List<City> list = new List<City>();
        //        var cs = context.Cities;
        //        foreach (var c in cs)
        //        {
        //            list.Add(new City
        //            {
        //                CityID = c.CityID,
        //                RegionRegionID = c.RegionRegionID,
        //                Name = c.Name
        //            });
        //        };
        //        return list;
        //    }
        //}

        public City AddCity(int CountryID, int RegionID, string NameNewCity)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Cities.Any(c => c.Name == NameNewCity))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Город '" +
                        NameNewCity + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }

                City newCity;
                newCity = context.Cities.Include("Regions").Include("Countries").Where(c => c.Name == NameNewCity).FirstOrDefault();
                if (newCity != null)
                {
                    if (!newCity.Regions.Any(r => r.RegionID == RegionID))
                    {
                        newCity.Regions.Add(context.Regions.Find(RegionID));
                    }
                }

                newCity = new City
                {
                    Name = NameNewCity,
                    Countries = new List<Country>(),
                    Regions = new List<Region>()
                };
                newCity.Countries.Add(context.Countries.Find(CountryID));
                if (RegionID != 0)
                {
                    newCity.Regions.Add(context.Regions.Find(RegionID));
                }
                context.Cities.Add(newCity);
                context.SaveChanges();
                return new City
                    {
                        CityID = newCity.CityID,
                        Name = newCity.Name,
                        FullName = newCity.FullNameFromDB
                    };
            }
        }

        public List<City> GetCities(int CountryID, int RegionID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<City> list = new List<City>();
                if (RegionID == 0)
                {
                    var c = context.Countries.Include("Cities")
                        .Where(cn => cn.CountryID == CountryID).FirstOrDefault();
                    var cts = c.Cities.OrderBy(ct => ct.Name);
                    foreach (var ct in cts)
                    {
                        list.Add(new City
                        {
                            CityID = ct.CityID,
                            FullName = ct.Name + ", " + c.Name
                        });
                    }
                }
                else
                {
                    var r = context.Regions.Include("Cities").Include("Countries")
                        .Where(rg => rg.RegionID == RegionID).FirstOrDefault();
                    var cts = r.Cities.OrderBy(ct => ct.Name);
                    foreach (var ct in cts)
                    {
                        list.Add(new City
                        {
                            CityID = ct.CityID,
                            FullName = ct.Name + ", " + r.FullNameFromDB
                        });
                    }
                }
                return list;
            }
        }

        public List<Region> GetRegions(int CountryID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Region> list = new List<Region>();
                var c = context.Countries.Include("Regions")
                    .Where(cn => cn.CountryID == CountryID).FirstOrDefault();
                var rs = c.Regions.OrderBy(r => r.Name);
                foreach (var r in rs)
                {
                    list.Add(new Region
                    {
                        RegionID = r.RegionID,
                        Name = r.Name,
                        FullName = r.Name + ", " + c.Name
                    });
                };
                return list;
            }
        }

        public Region AddRegion(int CountryID, string NewRegionName)
        {
            string newName = NewRegionName.Trim();
            if (newName.Length == 0)
            {
                CustomExpMsg customMsg = new CustomExpMsg("Не может быть пустой строкой!", 1);
                throw new FaultException<CustomExpMsg>(customMsg,
                    new FaultReason(customMsg.ErrorMsg));
            }
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                Country c = context.Countries.Include("Regions")
                    .Where(cn => cn.CountryID == CountryID).FirstOrDefault();
                List<Region> rs = c.Regions.ToList();
                if (rs.Any(r => r.Name == newName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Регион '" +
                        newName + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                Region newRegion = new Region
                {
                    Name = newName,
                    Countries = new List<Country>(),
                    Cities = new List<City>()
                    ////FullName = newName + ", " + c.Name
                };
                context.Regions.Add(newRegion);
                //context.SaveChanges();
                c.Regions.Add(newRegion);
                ////rs.Add(newRegion);
                context.SaveChanges();
                return new Region
                    {
                        RegionID = newRegion.RegionID,
                        Name = newRegion.Name,
                        FullName = newRegion.FullNameFromDB
                    };
            }
        }

        public void RenameRegion(int CountryID, int RegionID, string NewName)
        {
            string newName = NewName.Trim();
            if (newName.Length == 0)
            {
                CustomExpMsg customMsg = new CustomExpMsg("Не может быть пустой строкой!", 1);
                throw new FaultException<CustomExpMsg>(customMsg,
                    new FaultReason(customMsg.ErrorMsg));
            }
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                if (context.Countries.Include("Regions")
                    .Where(cn => cn.CountryID == CountryID).FirstOrDefault().Regions.Any(r => r.Name == NewName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Регион '" +
                        newName + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                context.Regions.Find(RegionID).Name = newName;
                context.SaveChanges();
            }
        }

        public void DelRegion(int RegionID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                Region region = context.Regions.Find(RegionID);
                context.Regions.Remove(region);
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    CustomExpMsg customMsg = new CustomExpMsg("'" + region.Name + "' используется!", 4);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
            }
        }

        public City AddCityToCountry(int CountryID, string NewCityName)
        {
            string newName = NewCityName.Trim();
            if (newName.Length == 0)
            {
                CustomExpMsg customMsg = new CustomExpMsg("Не может быть пустой строкой!", 1);
                throw new FaultException<CustomExpMsg>(customMsg,
                    new FaultReason(customMsg.ErrorMsg));
            }
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                var c = context.Countries.Include("Cities")
                    .Where(cn => cn.CountryID == CountryID).FirstOrDefault();
                var cs = c.Cities;
                if (cs.Any(r => c.Name == newName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Город '" +
                        newName + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                City newCity = new City
                {
                    Name = newName,
                    //FullName = newName + ", " + c.Name
                };
                cs.Add(newCity);
                context.SaveChanges();
                return newCity;
            }
        }

        public City AddCityToRegion(int RegionID, string NewCityName)
        {
            string newName = NewCityName.Trim();
            if (newName.Length == 0)
            {
                CustomExpMsg customMsg = new CustomExpMsg("Не может быть пустой строкой!", 1);
                throw new FaultException<CustomExpMsg>(customMsg,
                    new FaultReason(customMsg.ErrorMsg));
            }
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                var r = context.Regions.Include("Countries.Cities").Include("Cities")
                    .Where(rg => rg.RegionID == RegionID).FirstOrDefault();
                var cts = r.Cities;
                if (cts.Any(ct => ct.Name == newName))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Город '" +
                        newName + ", " + r.Name + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                City newCity;
                var cn = r.Countries.FirstOrDefault();
                if (cn.Cities.Any(ct => ct.Name == newName))
                {
                    //если город уже есть в стране, то только добавление в регион и возврат города
                    City city = cn.Cities.Where(c => c.Name == newName).FirstOrDefault();
                    r.Cities.Add(city);
                    newCity = new City
                    {
                        CityID = city.CityID,
                        Name = city.Name,
                        //FullName = city.Name + ", " + cn.Name
                    };
                }
                else
                {
                    //если города нет в стране, то добавление и в страну, и в регион, и также - его возврат
                    newCity = new City 
                    { 
                        Name = newName, 
                        //FullName = newName + ", " + cn.Name
                    };
                    context.Cities.Add(newCity);
                    cn.Cities.Add(newCity);
                    cts.Add(newCity);
                }
                context.SaveChanges();
                return newCity;
            }
        }

        public List<City> GetCitiesOfCountry(int CountryID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<City> list = new List<City>();
                var cn = context.Countries.Include("Cities")
                    .Where(c => c.CountryID == CountryID).FirstOrDefault().Cities;
                foreach (City ct in cn)
                {
                    list.Add(new City
                    {
                        CityID = ct.CityID,
                        Name = ct.Name
                    });
                }
                return list;
            }
        }

        public List<City> GetCitiesOfRegion(int RegionID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<City> list = new List<City>();
                var rg = context.Regions.Include("Cities").Include("Countries")
                    .Where(r => r.RegionID == RegionID).FirstOrDefault().Cities;
                foreach (var c in rg)
                {
                    list.Add(new City
                    {
                        CityID = c.CityID,
                        Name = c.Name
                    });
                }
                return list;
            }
        }

        //public Company GetCompanyUser(Guid SessionID)
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        Company user = new Company();
        //        var accountID = context.Sessions.Where(s => s.SessionID == SessionID).Select(s => s.AccountAccountID).FirstOrDefault();
        //        var personID = context.Accounts.Where(a => a.AccountID == accountID).Select(a => a.PersonPersonID).FirstOrDefault();
        //        var companyID = context.Persons.Where(p => p.PersonID == personID).Select(p => p.CompanyCompanyID).FirstOrDefault();


        //        return context.Companies.Where(c => c.CompanyID == companyID).FirstOrDefault();
        //    }
        //}

        int GetIdUserCompany(Guid SessionID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                var accountID = context.Sessions.Where(s => s.SessionID == SessionID).Select(s => s.AccountAccountID).FirstOrDefault();
                var personID = context.Accounts.Where(a => a.AccountID == accountID).Select(a => a.PersonPersonID).FirstOrDefault();
                return context.Persons.Where(p => p.PersonID == personID).Select(p => p.CompanyCompanyID).FirstOrDefault<int>();
            }
        }

//        public List<Company> GetFullSellers(Guid SessionID)
//        //public List<Company> GetFullSellers(int MyCompanyID)
//        {
//            using (ParametricDesignContext context = new ParametricDesignContext())
//            {
//                context.Database.CommandTimeout = 1800;
//                int? ttt = context.Database.CommandTimeout;
//                List<Company> list = new List<Company>();
//                try
//                {
//                    int myCompanyId = GetIdUserCompany(SessionID);
//                    var sellers = context.SellerCustomerCompanies.Include("Seller").
//                        Where(s => s.CustomerCompanyID == myCompanyId).Select(s => s.Seller);
//                    //var sellers = context.SellerCustomerCompanies.Include("Seller").
//                    //    Where(s => s.CustomerCompanyID == MyCompanyID).Select(s => s.Seller);
//                    foreach (var s in sellers)
//                    {
//                        Address legal = new Address
//                        {
//                            Street = s.LegalAddress.Street,
//                            House = s.LegalAddress.House,
//                            Office = s.LegalAddress.Office
//                        };
//                        Address actual = new Address
//                        {
//                            Street = s.ActualAddress.Street,
//                            House = s.ActualAddress.House,
//                            Office = s.ActualAddress.Office
//                        };

//                        Company cmp = new Company()
//                        {
//                            CompanyID = s.CompanyID,
//                            INN = s.INN,
//                            KPP = s.KPP,
//                            Name = s.Name,
//                            //LongName = s.LongName,
//                            LegalCityCityID = s.LegalCityCityID,

//                            //LegalAddress = legal,
//                            //ActualCityCityID = s.ActualCityCityID,
//                            //ActualAddress = actual
//                        };

//                        list.Add(cmp);
//                    }
//                    return list;
//;
//                }
//                catch
//                {
//                    return list;
//                }
//            }
//        }

        //public Dictionary<int, string> GetFullNameSellers(Guid SessionID)
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        int myCompanyId = GetIdUserCompany(SessionID);
        //        return context.SellerCustomerCompanies.Include("Seller").
        //            Where(s => s.CustomerCompanyID == myCompanyId).Select(s => s.Seller).
        //            Include("LegalCity").Include("LegalCity.Countries").ToDictionary(ss => ss.CompanyID, i => i.FullName);
        //    }
        //}
        public List<SellerCustomerCompany> GetFullSellers(Guid SessionID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<SellerCustomerCompany> list = new List<SellerCustomerCompany>();
                int myCompanyId = GetIdUserCompany(SessionID);
                //int myCompanyId = 1;
                List<SellerCustomerCompany> sellers = context.SellerCustomerCompanies
                    .Where(s => s.CustomerCompanyID == myCompanyId).ToList();
                foreach (SellerCustomerCompany s in sellers)
                {
                    list.Add(new SellerCustomerCompany
                    {
                        SellerCompanyID = s.SellerCompanyID,
                        Discount = s.Discount
                    });
                }
                return list;
            }
        }

        public List<Company> GetSmallSellers(Guid SessionID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Company> list = new List<Company>();
                int myCompanyId = GetIdUserCompany(SessionID);
                //int myCompanyId = 1;
                List<Company> sellers = context.SellerCustomerCompanies.Include("Seller").
                    Where(s => s.CustomerCompanyID == myCompanyId).Select(s => s.Seller).
                    Include("LegalCity").Include("LegalCity.Countries").ToList();
                foreach (Company s in sellers)
                {
                    list.Add(new Company
                    {
                        CompanyID = s.CompanyID,
                        INN = s.INN,
                        KPP = s.KPP,
                        Name = s.Name,
                        Currency = s.Currency,
                        LongName = s.LongName,
                        FullName = s.FullNameFromDB
                    });
                }
                return list;
            }
        }

        public List<Fitting> GetAllFittings(Guid SessionID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Fitting> list = new List<Fitting>();
                int myCompanyId = GetIdUserCompany(SessionID);
                List<Company> sellers = context.SellerCustomerCompanies.Include("Seller").
                    Where(s => s.CustomerCompanyID == myCompanyId).Select(s => s.Seller).ToList();
                foreach (var cmp in sellers)
                {
                    var fittings = context.Fittings.Where(f => f.CompanyCompanyID == cmp.CompanyID);
                    foreach (var ft in fittings)
                    {
                        list.Add(new Fitting
                        {
                            FittingID = ft.FittingID,
                            Article = ft.Article
                        });
                    }
                }
                return list;
            }
        }

        public List<Fitting> GetFittings(int CompanyID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<Fitting> list = new List<Fitting>();
                var fs = context.Fittings.Include("DimCount").Where(f => f.CompanyCompanyID == CompanyID);
                foreach (var f in fs)
                {
                    list.Add(new Fitting
                    {
                        FittingID = f.FittingID,
                        CompanyCompanyID = f.CompanyCompanyID,
                        Article = f.Article,
                        Name = f.Name,
                        DimCountDimID = f.DimCountDimID,
                        Price = f.Price,
                        FullName = f.Article + ", " + f.Name + ", " + f.DimCount.Name
                    });
                }
                return list;
            }
        }

        //public List<CombinationFitting> GetFittingsForCombination(int CombinationID)
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        List<CombinationFitting> list = new List<CombinationFitting>();
        //        var fs = context.CombinationFitting.Include("Fitting").Where(cf => cf.
        //        return list;
        //    }
        //}


        public Fitting AddFitting(int CompanyID, string Article, 
            string Name, int DimID, double Price)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                var fs = context.Fittings.Where(f => f.CompanyCompanyID == CompanyID);
                if (fs.Any(f => f.Article == Article))
                {
                    CustomExpMsg customMsg = new CustomExpMsg("Артикул '" +
                        Article + "' уже существует!", 1);
                    throw new FaultException<CustomExpMsg>(customMsg,
                        new FaultReason(customMsg.ErrorMsg));
                }
                Fitting newFitting = new Fitting
                {
                    CompanyCompanyID = CompanyID,
                    Article = Article,
                    Name = Name,
                    DimCountDimID = DimID,
                    Price = Price
                };
                context.Fittings.Add(newFitting);
                context.SaveChanges();
                return newFitting;
            }
        }

        bool CheckINN(string INN, out string Msg)
        {
            // является ли вообще числом 
            try
            {
                Int64.Parse(INN);
            }
            catch
            {
                Msg = "Недопустимый символ!";
                return false;
            }
            // проверка на кол-во символов - может быть только или 10, или 12 цифр
            if (INN.Length != 10 && INN.Length != 12) 
            {
                Msg = "Неверное количество символов!";
                return false; 
            }
            // проверка по контрольным цифрам
            if (INN.Length == 10) // для юридического лица
            {
                int dgt10 = 0;
                try
                {
                    dgt10 = (((2 * Int32.Parse(INN.Substring(0, 1))
                            + 4 * Int32.Parse(INN.Substring(1, 1))
                            + 10 * Int32.Parse(INN.Substring(2, 1))
                            + 3 * Int32.Parse(INN.Substring(3, 1))
                            + 5 * Int32.Parse(INN.Substring(4, 1))
                            + 9 * Int32.Parse(INN.Substring(5, 1))
                            + 4 * Int32.Parse(INN.Substring(6, 1))
                            + 6 * Int32.Parse(INN.Substring(7, 1))
                            + 8 * Int32.Parse(INN.Substring(8, 1))) % 11) % 10);
                }
                catch
                {
                    Msg = "Неверное контрольное число!";
                    return false;
                }
                if (Int32.Parse(INN.Substring(9, 1)) == dgt10)
                {
                    Msg = null;
                    return true;
                }
                else
                {
                    Msg = "Неверная контрольная сумма!";
                    return false;
                }
            }
            else
            {
                Msg = "12 символов может быть только в ИНН физического лица!";
                return false;
            }
        }

        bool CheckKPP(string KPPstring)
        {
            //return new Regex(@"\d{4}[\dA-Z][\dA-Z]\d{3}").IsMatch(KPPstring);
            return true;
        }

        public Company AddCompany(string INN, string KPP,
            string LongName, string Name,
            int LegalCityID, Address LegalAddress,
            int ActualCityID, Address ActualAddress)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                Company newCompany = null;
                newCompany = new Company
                {
                    INN = INN,
                    KPP = KPP,
                    LongName = LongName,
                    Name = Name,
                    LegalCityCityID = LegalCityID,
                    LegalAddress = new Address
                    {
                        Street = LegalAddress.Street,
                        House = LegalAddress.House,
                        Office = LegalAddress.Office
                    },
                    ActualCityCityID = ActualCityID,
                    ActualAddress = new Address
                    {
                        Street = ActualAddress.Street,
                        House = ActualAddress.House,
                        Office = ActualAddress.Office
                    }
                };
                context.Companies.Add(newCompany);
                try
                {
                    context.SaveChanges();
                }
                catch (UpdateException ex)
                {

                }
                return newCompany;
            }
        }

        //public Company AddSeller(int UserCompanyID, string INN, string KPP,
        //    string LongName, string Name,
        //    int LegalCityID, Address LegalAddress,
        //    int ActualCityID, Address ActualAddress, int Discount)
        //{
        //    using (ParametricDesignContext context = new ParametricDesignContext())
        //    {
        //        int userCompanyID = UserCompanyID;
        //        Company newSeller = null;
        //        Company existCompany = null, existSeller = null;
        //        string msg;
        //        if (!CheckINN(INN, out msg))
        //        {
        //            CustomExpMsg customMsg = new CustomExpMsg(msg, 1);
        //            throw new FaultException<CustomExpMsg>(customMsg,
        //                new FaultReason(customMsg.ErrorMsg));
        //        }
        //        if (!CheckKPP(KPP))
        //        {
        //            CustomExpMsg customMsg = new CustomExpMsg("Неверный КПП!", 1);
        //            throw new FaultException<CustomExpMsg>(customMsg,
        //                new FaultReason(customMsg.ErrorMsg));
        //        }
        //        //var fs = context.Fittings.Where(f => f.CompanyCompanyID == CompanyID);
        //        //if (fs.Any(f => f.Article == Article))
        //        //userCompanyID = GetIdUserCompany(SessionID); //Оставить!!!!!!!!!!!!!!
        //        if(context.Companies.Any(c => c.INN == INN))
        //        {
        //            var csINN = context.Companies.Where(c => c.INN == INN);
        //            if(csINN.Any(c => c.KPP == KPP))
        //            {
        //                existCompany = csINN.Where(c => c.KPP == KPP).First<Company>();
        //                if(context.SellerCustomerCompanies
        //                    .Where(scc => scc.CustomerCompanyID == userCompanyID)
        //                    .Any(c => c.SellerCompanyID == newSeller.CompanyID))
        //                {
        //                    existSeller = existCompany;
        //                }
        //            }
        //        }

        //        if (existCompany != null)
        //        {
        //            if (existSeller != null)
        //            {
        //                return existSeller;
        //            }
        //            else
        //            {
        //                context.SellerCustomerCompanies.Add(new SellerCustomerCompany
        //                {
        //                    CustomerCompanyID = userCompanyID,
        //                    SellerCompanyID = existCompany.CompanyID,
        //                    Discont = Discount
        //                });
        //            }
        //        }

        //        //try
        //        //{
        //        //    var csINN = context.Companies.Where(c => c.INN == INN);
        //        //    newSeller = csINN.Where(c => c.KPP == KPP).First<Company>();
        //        //    if(context.SellerCustomerCompanies
        //        //        .Where(scc => scc.CustomerCompanyID == userCompanyID)
        //        //        .Any(c => c.SellerCompanyID == cKPP.CompanyID))
        //        //    {

        //        //    }
        //        //    newSeller = new Company
        //        //    {
        //        //         INN = cKPP.INN,
        //        //         KPP = 
        //        //}
        //        //catch
        //        //{

        //        //}
        //        //if (context.Companies.Any(c => c.INN == INN))
        //        //{
        //        //    var cs = context.Companies.Where(c => c.INN == 
        //        //    if(context.Companies.Any(c => c.KPP == KPP))
        //        //    {

        //        //    }
        //        //}
        //        return newSeller;
        //    }
        //}

        public int FindCompanyID(string INN, string KPP)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                try
                {
                    return context.Companies.Where(c => c.INN == INN && c.KPP == KPP).Select(c => c.CompanyID).First();
                }
                catch
                {
                    return 0;
                }
            }
        }

        public bool AddSellerFromExistCompany(Guid SessionID, int CompanyID, int Discount)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.SellerCustomerCompanies.Add(new SellerCustomerCompany
                {
                    CustomerCompanyID = GetIdUserCompany(SessionID),
                    SellerCompanyID = CompanyID,
                    Discount = Discount
                });
                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<City> GetCitiesAsName(string Name)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                List<City> list = new List<City>();
                List<City> cities = context.Cities.Include("Countries").Include("Regions").Where(c => c.Name == Name).ToList();
                foreach (City city in cities)
                {
                    City ct = new City
                    {
                        CityID = city.CityID,
                        FullName = city.FullNameFromDB,
                        Name = city.Name,
                        Regions = new List<Region>(),
                        Countries = new List<Country>()
                    };
                    foreach (Region rg in city.Regions)
                    {
                        ct.Regions.Add(new Region
                        {
                            RegionID = rg.RegionID,
                            Name = rg.Name,
                            FullName = rg.FullNameFromDB
                        });
                    }
                    foreach (Country cn in city.Countries)
                    {
                        ct.Countries.Add(new Country
                        {
                            CountryID = cn.CountryID,
                            Name = cn.Name
                        });
                    }
                    list.Add(ct);
                }
                return list;
            }
        }

        public City AddCityAsName(string Name)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                City newCity = new City 
                { 
                    Name = Name,
                    Countries = new List<Country>(),
                    Regions = new List<Region>()
                };
                context.Cities.Add(newCity);
                context.SaveChanges();
                return newCity;
            }
        }

        public City AddCityToRegion(City City, Region Region)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                City.Regions.Add(Region);
                context.SaveChanges();
                return City;
            }
        }

        public City AddCityToCountry(int CityID, int CountryID)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                City city = context.Cities.Include("Countries").Where(c => c.CityID == CityID).FirstOrDefault();
                city.Countries.Add(context.Countries.Find(CountryID));
                context.SaveChanges();
                return city;
            }
        }

        public Country AddNewCountry(string NameNewCountry)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                Country newCountry = context.Countries
                    .Where(cn => cn.Name == NameNewCountry).FirstOrDefault();
                if (newCountry == null)
                {
                    newCountry = new Country
                    {
                        Name = NameNewCountry,
                        Regions = new List<Region>(),
                        Cities = new List<City>()
                    };
                    context.Countries.Add(newCountry);
                    context.SaveChanges();
                }
                return new Country
                {
                    CountryID = newCountry.CountryID,
                    Name = newCountry.Name
                };
            }
        }

        public Region AddNewRegion(int CountryID, string NameNewRegion)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                Country country = context.Countries.Include(cn => cn.Regions)
                    .Where(cn => cn.CountryID == CountryID).FirstOrDefault();
                Region newRegion = country.Regions.Where(r => r.Name == NameNewRegion).FirstOrDefault();
                if (newRegion == null)
                {
                    newRegion = new Region
                    {
                        Name = NameNewRegion,
                        Countries = new List<Country>() { country },
                        Cities = new List<City>()
                    };
                    context.Regions.Add(newRegion);
                    context.SaveChanges();
                }
                return new Region
                {
                    RegionID = newRegion.RegionID,
                    Name = newRegion.Name,
                    FullName = newRegion.Name + ", " + country.Name
                };
            }
        }

        public City AddNewCity(int CountryID, int RegionID, string NameNewCity)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                Country country = context.Countries.Include(cn => cn.Cities)
                    .Where(cn => cn.CountryID == CountryID).FirstOrDefault();
                Region region = context.Regions.Include(r => r.Cities)
                    .Where(r => r.RegionID == RegionID).FirstOrDefault();
                City newCity = country.Cities.Where(c => c.Name == NameNewCity).FirstOrDefault();
                if (newCity == null)
                {
                    newCity = new City
                    {
                        Name = NameNewCity,
                        Countries = new List<Country>() { country },
                        Regions = new List<Region>()
                    };
                    if (region != null) newCity.Regions.Add(region);
                    context.Cities.Add(newCity);
                }
                else
                {
                    if (region != null)
                    {
                        if (!region.Cities.Contains(newCity))
                        {
                            region.Cities.Add(newCity);
                        }
                    }
                }
                context.SaveChanges();
                return new City
                {
                    CityID = newCity.CityID,
                    Name = newCity.Name,
                    //FullName = newCity.FullNameFromDB
                    FullName = newCity.Name + ", " + 
                    ((region != null) ? (region.Name + ", ") : "") + 
                    country.Name
                };
            }
        }

        public Company AddNewCompany(Company NewCompany)
        {
            using (ParametricDesignContext context = new ParametricDesignContext())
            {
                context.Companies.Add(NewCompany);
                context.SaveChanges();
                return NewCompany;

            }

        }

        public double GetRateToRUB(string CodeCurrency, string ShortDate)
        {
            WebResponse response = null;
            StreamReader strReader = null;
            try
            {
                WebRequest request = WebRequest.Create(new Uri(@"http://www.cbr.ru/scripts/xml_daily.asp?date_req=" +
                    ShortDate));
                response = request.GetResponse();
                strReader = new StreamReader(response.GetResponseStream());
                string line = strReader.ReadToEnd();
                response.Close();

                XmlDocument doc1 = new XmlDocument();

                doc1.LoadXml(line);
                XmlNode rootNode = doc1.DocumentElement;

                return XmlConvert.ToDouble(rootNode
                    .SelectSingleNode("Valute[CharCode = '" + CodeCurrency + "']/Value")
                    .InnerText.Replace(",", "."));
            }
            catch (Exception ex)
            {
                return 0d;
            }
        }

        public double GetRateToEUR(string CodeCurrency)
        {
            WebResponse response = null;
            StreamReader strReader = null;
            try
            {
                WebRequest request = WebRequest.Create(new Uri(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"));
                response = request.GetResponse();
                strReader = new StreamReader(response.GetResponseStream());
                string line = strReader.ReadToEnd();
                response.Close();

                XmlDocument doc1 = new XmlDocument();

                doc1.LoadXml(line);


                XmlNode rootNode = doc1.DocumentElement.ChildNodes[2].ChildNodes[0];
                //price[@intl = "Canada"]

                //IEnumerable<XElement> cube =
                //    from el in rootNode.ChildNodes
                //    where (string)el.Attribute("currency") == CodeCurrency
                //    select el;
                //var elements = from elmts in xDoc.Descendants()
                //               where elmts.Attribute(txtAtrName.Text).Value == txtAtrValue.Text
                //               select elmts;

                XmlNodeList ll = rootNode.SelectNodes("/Cube"); 
                XmlNode cc = rootNode.FirstChild;
                XmlNode nnn = rootNode.SelectSingleNode("Cube");
                XmlNode nn = rootNode.SelectSingleNode("Cube/Cube/Cube[@currency='RUB']");
                //return XmlConvert.ToDouble(rootNode
                //    .SelectSingleNode("Cube[currency = '" + CodeCurrency + "']/rate")
                //    .InnerText.Replace(",", "."));

                //XmlNamespaceManager ns = new XmlNamespaceManager(doc1.NameTable);
                //ns.AddNamespace("msbld", "http://schemas.microsoft.com/developer/msbuild/2003");
                //XmlNode node = xmldoc.SelectSingleNode("//msbld:Compile", ns);
                //XmlNamespaceManager ns = new XmlNamespaceManager(xmldoc.NameTable);
                //ns.AddNamespace("msbld", "http://schemas.microsoft.com/developer/msbuild/2003");
                //XmlNode node = xmldoc.SelectSingleNode("//msbld:Compile", ns);
                //http://stackoverflow.com/questions/4171451/xmldocument-selectsinglenode-and-xmlnamespace-issue

                return 44d;
            }
            catch (Exception ex)
            {
                return 0d;
            }


        }

        public double GetRate(string FromCurrency, string ToCurrency)
        {
            WebRequest webrequest = WebRequest.Create("http://www.webservicex.net/CurrencyConvertor.asmx/ConversionRate?FromCurrency=USD&ToCurrency=INR");
            HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseFromServer);
            string value = doc.InnerText;
            //MessageBox.Show(value);
            reader.Close();
            dataStream.Close();
            response.Close();
            //string xmlResult = null;
            //string url;
            //url = "http://www.webservicex.net/CurrencyConvertor.asmx/ConversionRate?FromCurrency=" + FromCurrency + "&ToCurrency=" + ToCurrency + "";
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //StreamReader resStream = new StreamReader(response.GetResponseStream());
            //XmlDocument doc = new XmlDocument();
            //xmlResult = resStream.ReadToEnd();
            //doc.LoadXml(xmlResult);
            ////Label1.Text = "Current Exchange Rate for " + TextBox1.Text.ToUpper() + " ---> " + TextBox2

            return 0d;
        }

        //public static List<KeyValuePair<string, decimal>> GetCurrencyListFromWeb(out DateTime currencyDate)
        //{
        //    List<KeyValuePair<string, decimal>> returnList = new List<KeyValuePair<string, decimal>>();
        //    string date = string.Empty;
        //    using (XmlReader xmlr = XmlReader.Create(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"))
        //    {
        //        xmlr.ReadToFollowing("Cube");
        //        while (xmlr.Read())
        //        {
        //            if (xmlr.NodeType != XmlNodeType.Element) continue;
        //            if (xmlr.GetAttribute("time") != null)
        //            {
        //                date = xmlr.GetAttribute("time");
        //            }
        //            else returnList.Add(new KeyValuePair<string, decimal>(xmlr.GetAttribute("currency"), decimal.Parse(xmlr.GetAttribute("rate"), CultureInfo.InvariantCulture)));
        //        }
        //        currencyDate = DateTime.Parse(date);
        //    }
        //    returnList.Add(new KeyValuePair<string, decimal>("EUR", 1));
        //    return returnList;
        //}


        private double evalrpn(Stack<string> tks)
        {
            string tk = tks.Pop();
            double x, y;
            if (!Double.TryParse(tk, out x))
            {
                y = evalrpn(tks); x = evalrpn(tks);
                if (tk == "+") x += y;
                else if (tk == "-") x -= y;
                else if (tk == "*") x *= y;
                else if (tk == "/") x /= y;
                else throw new Exception();
            }
            return x;
        }

        private string[] GetRPN(string[] strArray)
        {
            string result = string.Empty;
            Stack<char> stack = new Stack<char>();
            foreach (string st in strArray)
            {
                double dd;
                if (double.TryParse(st, out dd))//если это число - в выходную строку
                {
                    result += st + " ";
                }
                else
                {

                }
            }


            return result.Trim().Split(new char[] { ' ' });
        }

        public float GetValueExpression(int TypeProductID, string Expression)
        {
            string str = Expression;


            //char[] sp = new char[] { ' ', '\t' };
            //Stack<string> tks = new Stack<string>
            //     (Expression.Split(sp, StringSplitOptions.RemoveEmptyEntries));

            string separator = System.Globalization.CultureInfo.CurrentCulture
                .NumberFormat.CurrencyDecimalSeparator;
            //string[] strArr = GetArray(Expression);

            str = str.Replace("(-", "(0-");
            str = str.Replace(".", separator);
            //string[] arrStr = Regex.Matches(str, @"(\#|\+|\-|\*|\/|\^|\(|\)|(\d+\" + separator + @"?\d*))")
            string[] arrStr = Regex.Matches(str, @"(\#|\+|\-|\*|\/|\(|\)|(\d+\" + separator + @"?\d*))")
                .Cast<Match>()
                .Select(match => match.Value)
                .ToArray();

            //Stack<string> tks = new Stack<string>(arrStr);
            //double d = evalrpn(tks);
            float ff;
            if (arrStr.Length == 1 && float.TryParse(arrStr[0], out ff)) return ff;
            if (str.Contains("#"))
            {
                //string[] strArr1 = ReplaceVariable(strArr);
                Dictionary<int, int> dictValuePrm = new Dictionary<int, int>();
                using (ParametricDesignContext context = new ParametricDesignContext())
                {
                    var tpps = context.TypeProductParameters.Where(tpp => tpp.TypeProductTypeProductID == TypeProductID);
                    foreach (var tpp in tpps)
                    {
                        dictValuePrm.Add(tpp.ParameterParameterID, tpp.DefaultValue);
                    }

                }
                List<string> strList = new List<string>();
                for (int i = 0; i < arrStr.Length; i++)
                {
                    if (arrStr[i] == "#")
                    {
                        i++;
                        int idPrm;
                        if (int.TryParse(arrStr[i], out idPrm))
                        {
                            strList.Add(dictValuePrm[idPrm].ToString());
                        }
                    }
                    else
                    {
                        strList.Add(arrStr[i]);
                    }
                }
                arrStr = strList.ToArray<string>();
                //string[] strArr1 = strList.ToArray<string>();
            }
            //string[] rpnArr = GetRPN(strArr1);

            string result = string.Empty; //выходная строка
            Stack<char> stack = new Stack<char>();
            foreach (string st in arrStr)//Получаем перебором текущий член массива
            {
                double dd;
                if (double.TryParse(st, out dd))//если это число - в выходную строку
                {
                    result += st + " ";
                }
                else
                {
                    char ch = st.ToCharArray(0, 1)[0];
                    if (GetPriority(ch) > 1)//если знак операции
                    {
                        if (stack.Count == 0)
                        {
                            stack.Push(ch);//в стек
                        }
                        else
                        {
                            if (GetPriority(stack.Peek()) < GetPriority(ch) || stack.Count == 0)
                            {
                                stack.Push(ch);
                            }
                            else
                            {
                                while (stack.Count > 0 && GetPriority(stack.Peek()) >= GetPriority(ch))
                                {
                                    result += stack.Pop().ToString() + " ";
                                }
                                stack.Push(ch);//!!!!!!!!!!!!!!!!!новая строка - проверить
                            }
                        }
                    }
                    else if (ch == '(')
                    {
                        //если очередной член архива открывающая скобка - в стек
                        stack.Push(ch);
                    }
                    else//осталась возможна только закрывающий скобка
                    {
                        while (stack.Peek() != '(')
                        {
                            result += stack.Pop().ToString() + " ";
                        }
                        stack.Pop();
                    }
                }
            }
            while (stack.Count > 0)
            {
                result += stack.Pop().ToString() + " ";
            }

            string[] rpnArr = result.Trim().Split(new char[] { ' ' });

            //float result = GetResult(rpnArr);

            Stack<float> stack1 = new Stack<float>();
            foreach (string st in rpnArr)
            {
                float dd, d2;
                if (float.TryParse(st, out dd))//если это число - в выходную строку
                {
                    stack1.Push(dd);
                }
                else
                {
                    switch (st)
                    {
                        case "+":
                            d2 = stack1.Pop();
                            stack1.Push(stack1.Pop() + d2);
                            break;
                        case "-":
                            d2 = stack1.Pop();
                            stack1.Push(stack1.Pop() - d2);
                            break;
                        case "*":
                            d2 = stack1.Pop();
                            stack1.Push(stack1.Pop() * d2);
                            break;
                        case "/":
                            d2 = stack1.Pop();
                            stack1.Push(stack1.Pop() / d2);
                            break;
                        //case "^":
                        //    d2 = stack1.Pop();
                        //    float d3 = stack1.Pop();
                        //    stack1.Push((int)d3 ^ (int)d2);
                        //    break;
                        default:
                            break;
                    }
                }

            }
            if (stack1.Count == 1)
            {
                return stack1.Pop();
            }
            else
            {
                //MessageBox.Show("В стеке " + stack.Count.ToString() + ".\nА должно быть 1");
                return 0f;
            }
        }

        byte GetPriority(char s)
        {
            switch (s)
            {
                case '(':
                case ')':
                    return 1;
                case '+':
                case '-':
                    return 2;
                case '*':
                case '/':
                    return 3;
                default:
                    return 4;
            }
        }

        //string[] GetArray(string str)
        //{
        //    str = str.Replace("(-", "(0-");
        //    str = str.Replace(".", separator);
        //    return Regex.Matches(str, @"(\#|\+|\-|\*|\/|\(|\)|(\d+\" + separator + @"?\d*))")
        //        .Cast<Match>()
        //        .Select(match => match.Value)
        //        .ToArray();
        //}

    }
}
