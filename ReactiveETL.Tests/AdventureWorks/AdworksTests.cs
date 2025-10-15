namespace ReactiveETL.Tests.AdventureWorks;

using System;
using System.Configuration;
using System.Data;

/// <summary>
/// Summary description for UnitTest1
/// </summary>

public class AdworksTests
{
    [Fact]
    public void AdventureWork()
    {
        string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
            
            
        Input.NonQuery("output", CleanCatalog).Execute();

        var famillies = Input.Query("advwrk", "Select * from dimproductcategory").Named("SelectFamillies")
            .Transform(r => r.Set("fam_id", Guid.NewGuid()))
            .DbCommand("output", InsertFamilly)
            .Execute();

        var categories = Input.Query("advwrk", "Select * from dimproductsubcategory").Named("SelectCategories")
            .Transform(r => r.Set("cat_id", Guid.NewGuid()))
            .Join(
                famillies, 
                (rowcat, rowfam) => rowcat["ProductCategoryKey"].Equals(rowfam["ProductCategoryKey"]),
                (rowcat, rowfam) => { rowcat["fam_id"] = rowfam["fam_id"]; return rowcat; })
            .DbCommand("output", InsertCategory)
            .Execute();

        var products = Input.Query("advwrk", "Select * from dimproduct").Named("SelectProducts")
            .Transform(r => r.Set("product_id", Guid.NewGuid()))
            .Join(
                categories,
                (rowprd, rowcat) => rowcat["ProductSubcategoryKey"].Equals(rowprd["ProductSubcategoryKey"]),
                (rowprd, rowcat) => { rowprd["cat_id"] = rowcat["cat_id"]; return rowprd; })
            .DbCommand("output", InsertProduct).Record().Named("RecProducts");

        var productImage = products
            .Filter(row => row["LargePhoto"] != null)
            .Record().Named("RecImages");
        var productCategory = products
            .Filter(HasCategory).Named("FilterCategory")
            .DbCommand("output", InsertProductCategory).Record().Named("RecCategories");

        products.Execute(true);

        Console.WriteLine("num products " + products.Result.Count);
        Console.WriteLine("num products images " + productImage.Result.Count);
        Console.WriteLine("num products images " + productCategory.Result.Count);
        products.Result.Count.ShouldBeGreaterThan(0);
        productImage.Result.Count.ShouldBeGreaterThan(0);
        productCategory.Result.Count.ShouldBeGreaterThan(0);
    }

    private bool HasCategory(Row row) => row["cat_id"] != null && !string.IsNullOrEmpty(row["cat_id"].ToString());

    private string CleanCatalog => "DELETE FROM ProductToCategories; " +
                                   "DELETE FROM Products; " +
                                   "DELETE FROM ProductCategories; " +
                                   "DELETE FROM ProductFamillies; ";

    private void InsertFamilly(IDbCommand cmd, Row row)
    {
        cmd.CommandText =
            @"INSERT INTO ProductFamillies (FamillyId,Title,UrlCode,Description) VALUES (@fam_id,@fam_title, @fam_url,@fam_desc)";

        cmd.AddParameter("fam_id", row["fam_id"]);
        cmd.AddParameter("fam_title", row["FrenchProductCategoryName"]);
        cmd.AddParameter("fam_desc", "Lorem Ipsum dolor...");
        cmd.AddParameter("fam_url", Guid.NewGuid().ToString());
    }

    private void InsertCategory(IDbCommand cmd, Row row)
    {
        cmd.CommandText =
            @"INSERT INTO ProductCategories (CategoryId,Familly_Id, Title,UrlCode,Description) VALUES (@id, @idfamille, @title, @url,@desc)";

        cmd.AddParameter("id", row["cat_id"]);
        cmd.AddParameter("idfamille", row["fam_id"]);
        cmd.AddParameter("title", row["FrenchProductSubcategoryName"]);
        cmd.AddParameter("desc", "Lorem Ipsum dolor...");
        cmd.AddParameter("url", Guid.NewGuid().ToString());
    }

    private void InsertProduct(IDbCommand cmd, Row row)
    {
        cmd.CommandText =
            @"INSERT INTO Products (ProductId, Title,UrlCode,Description, PriceValue, PriceCurrency) VALUES (@id, @title, @url,@desc, @priceval, @pricecurr)";

        row["product_id"] = Guid.NewGuid();

        cmd.AddParameter("id", row["product_id"]);
        cmd.AddParameter("idcat", row["cat_id"]);
        cmd.AddParameter("priceval", row["ListPrice"]);
        cmd.AddParameter("pricecurr", "EUR");
        string productName = row["FrenchProductName"].ToString();
        if (string.IsNullOrEmpty(productName))
            productName = row["EnglishProductName"].ToString();
        cmd.AddParameter("title", productName);
        cmd.AddParameter("desc", row["FrenchDescription"]);
        cmd.AddParameter("url", Guid.NewGuid().ToString());
    }

    private void InsertProductCategory(IDbCommand cmd, Row row)
    {
        cmd.CommandText =
            @"INSERT INTO ProductToCategories (ProductId,CategorySnapshotId) VALUES (@productid, @catid)";

        cmd.AddParameter("productid", row["product_id"]);
        cmd.AddParameter("catid", row["cat_id"]);
    }
}