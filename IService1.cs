using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using ParametricDesignWcfServiceLibrary.Model;

namespace ParametricDesignWcfServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        //[OperationContract]
        //int AddCompany(string Name);
        //[OperationContract]
        //int AddCustomer(int SellerID, string Name);
        [OperationContract]
        List<Company> GetCustomers(int SellerID);

        //[OperationContract]
        //Company GetCompanyUser(Guid SessionID);
        [OperationContract]
        string GetCompanyName(int Id);
        [OperationContract]
        Company GetCompany(int Id);
        [OperationContract]
        List<Company> GetAllCompanies();
        [OperationContract]
        List<Person> GetPersonsOfCompany(int Id);
        [OperationContract]
        void AddPersonToCompany(int Id, string Name);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        Guid GetSession(string UserName, string Password, bool WithRegistration);
        [OperationContract]
        void CloseSession(Guid SessionGuid);
        [OperationContract]
        int AddType(Guid Session, int? ParentTypeID, string NameNewTypeProduct);
        [OperationContract]
        void RenameType(Guid Session, int TypeID, string NewName);
        [OperationContract]
        List<TypeProduct> GetTypes(Guid Session, int? ParentTypeID);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void DelType(Guid Session, int TypeID);
        [OperationContract]
        List<TypeProductParameter> GetTypeParameters(Guid Session, int TypeID);
        [OperationContract]
        TypeProduct GetType(int TypeID);
        [OperationContract]
        List<Parameter> GetParameters();
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        int AddParameter(Parameter NewParameter);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void RenameParameter(Parameter CurParameter);
        [OperationContract]
        TypeProductParameter AddTypeParameter(int TypeID, int ParamID, int NewDefaultValue);
        [OperationContract]
        List<Combination> GetCombinations(Guid Session, int TypeID);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        Combination AddCombination(int TypeID, string NewName);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void RenameCombination(int TypeID, int CombinationID, string NewName);
        [OperationContract]
        void DelCombination(Guid Session, int CombinationID);
        [OperationContract]
        List<CombinationParameter> GetCombinationParameters(int CombinationID);
        //[OperationContract]
        //CombinationParameter AddCombinationParameter();
        [OperationContract]
        List<Dim> GetDims();
        [OperationContract]
        List<CombinationFitting> GetCombinationFittings(int CombinationID);
        [OperationContract]
        CombinationFitting AddCombinationFitting(int CombinationID, int FittingID, 
                                                string Name, int DimCountID, string Count, 
                                                int DimSizeID, string Size);
        [OperationContract]
        int AddCombinationFitting_1(CombinationFitting CombinationFitting);
        [OperationContract]
        void EditCombinationFitting(CombinationFitting CombinationFitting);
        [OperationContract]
        void DelCombinationFitting(int CombinationFittingID);
        [OperationContract]
        string GetArticle(int FittingID);
        //[OperationContract]
        //List<Fitting> GetFittings(int SessionID);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        Dim AddDim(string NewDimName);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void RenameDim(int DimID, string NewName);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void DelDim(int DimID);
        [OperationContract]
        List<Country> GetCountries();
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        Country AddCountry(string NewCountryName);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void RenameCountry(int CountryID, String NewName);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void DelCountry(int CountryID);
        //[OperationContract]
        //List<City> GetSities();
        [OperationContract]
        List<Region> GetRegions(int CountryID);
        [OperationContract]
        List<City> GetCities(int CountryID, int RegionID);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        Region AddRegion(int CountryID, string NewRegionName);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void RenameRegion(int CountryID, int RegionID, string NewName);
        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        void DelRegion(int RegionID);
        [OperationContract]
        List<City> GetCitiesOfCountry(int CountryID);
        [OperationContract]
        List<City> GetCitiesOfRegion(int RegionID);

        [OperationContract]
        List<Company> GetSmallSellers(Guid SessionID);
        [OperationContract]
        List<SellerCustomerCompany> GetFullSellers(Guid SessionID);
        //[OperationContract]
        //Dictionary<int, string> GetFullNameSellers(Guid SessionID);


        [OperationContract]
        List<Fitting> GetFittings(int CompanyID);

        [OperationContract]
        List<Fitting> GetAllFittings(Guid SessionID);

        //[OperationContract]
        //List<Fitting> GetFittingsForCombination(int CombinationID);

        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        Fitting AddFitting(int CompanyID, string Article, string Name, int DimID, double Price);

        //[OperationContract]
        //[FaultContract(typeof(CustomExpMsg))]
        //Company AddSeller(
        //    //Guid SessionID, 
        //    int UserCompanyID,
        //    string INN,
        //    string KPP,
        //    string LongName,
        //    string Name,
        //    int LegalCityID,
        //    Address LegalAddress,
        //    int ActualCityID,
        //    Address ActualAddress,
        //    int Discount);

        [OperationContract]
        [FaultContract(typeof(CustomExpMsg))]
        Company AddCompany(
            string INN,
            string KPP,
            string LongName,
            string Name,
            int LegalCityID,
            Address LegalAddress,
            int ActualCityID,
            Address ActualAddress
        );

        [OperationContract]
        int FindCompanyID(string INN, string KPP);

        [OperationContract]
        bool AddSellerFromExistCompany(Guid SessionID, int CompanyID, int Discount);

        [OperationContract]
        City AddCity(int CountryID, int RegionID, string NameNewCity);

        [OperationContract]
        List<City> GetCitiesAsName(string Name);

        [OperationContract]
        City AddCityAsName(string Name);

        [OperationContract]
        City AddCityToRegion(City City, Region Region);

        [OperationContract]
        City AddCityToCountry(int CityID, int CountryID);

        [OperationContract]
        City AddNewCity(int CountryID, int RegionID, string NameNewCity);

        [OperationContract]
        Region AddNewRegion(int CountryID, string NameNewRegion);

        [OperationContract]
        Country AddNewCountry(string NameNewCountry);

        [OperationContract]
        Company AddNewCompany(Company NewCompany);

        [OperationContract]
        double GetRateToRUB(string CodeCurrency, string ShortDate);

        [OperationContract]
        double GetRateToEUR(string CodeCurrency);

        [OperationContract]
        double GetRate(string FromCurrency, string ToCurrency);

        [OperationContract]
        List<TypeProductParameter> GetTypeProductParametersFromCombinationID(int CombinationID);

        [OperationContract]
        float GetValueExpression(int TypeProductID, string Expression);
    }
}
