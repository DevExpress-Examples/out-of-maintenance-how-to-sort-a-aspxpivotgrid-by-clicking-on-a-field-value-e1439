<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="DevExpress.Web.ASPxPivotGrid.v21.2, Version=21.2.11.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web.ASPxPivotGrid" TagPrefix="dxwpg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <dxwpg:ASPxPivotGrid ID="ASPxPivotGrid1" runat="server" DataSourceID="SqlDataSource1" ClientInstanceName="ASPxPivotGrid1" OnCustomCallback="ASPxPivotGrid1_CustomCallback" ClientIDMode="AutoID" IsMaterialDesign="False">
            <Fields>
                <dxwpg:PivotGridField ID="fieldCategoryName" Area="RowArea" AreaIndex="0">
                    <DataBindingSerializable>
                        <dxwpg:DataSourceColumnBinding ColumnName="CategoryName" />
                    </DataBindingSerializable>
                </dxwpg:PivotGridField>
                <dxwpg:PivotGridField ID="fieldProductName" Area="RowArea" AreaIndex="1">
                    <DataBindingSerializable>
                        <dxwpg:DataSourceColumnBinding ColumnName="ProductName" />
                    </DataBindingSerializable>
                </dxwpg:PivotGridField>
                <dxwpg:PivotGridField ID="field" Area="DataArea" AreaIndex="0" 
                    Caption="Sales">
                    <DataBindingSerializable>
                        <dxwpg:DataSourceColumnBinding ColumnName="ProductSales" />
                    </DataBindingSerializable>
                </dxwpg:PivotGridField>
                <dxwpg:PivotGridField ID="fieldSalesCount" Area="DataArea" AreaIndex="1" 
                    Caption="Count" SummaryType="Count">
                    <DataBindingSerializable>
                        <dxwpg:DataSourceColumnBinding ColumnName="ProductSales" />
                    </DataBindingSerializable>
                </dxwpg:PivotGridField>
                <dxwpg:PivotGridField ID="fieldShippedDate" Area="ColumnArea" AreaIndex="0">
                    <DataBindingSerializable>
                        <dxwpg:DataSourceColumnBinding ColumnName="ShippedDate" GroupInterval="DateYear"/>
                    </DataBindingSerializable>
                </dxwpg:PivotGridField>
            </Fields>
            <OptionsData DataProcessingEngine="Optimized" />
        </dxwpg:ASPxPivotGrid>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:ConnectionString %>" ProviderName="<%$ ConnectionStrings:ConnectionString.ProviderName %>" SelectCommand="SELECT * FROM [ProductReports]"></asp:SqlDataSource>
    
    </div>
    </form>
</body>
</html>
