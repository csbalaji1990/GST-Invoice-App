USE [SimpleGstInvoicer]
GO
/****** Object:  StoredProcedure [dbo].[ClearData]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[ClearData]
as
begin

delete from DbBankAccount
delete from DbBankTransaction
delete from DbCustomer
delete from DbCustomerLedger
delete from DbDeposit
delete from DbExpense
delete from DbItem
delete from DbPurchaseInvoiceHeader
delete from DbPurchaseInvoiceHsnSacDetails
delete from DbPurchaseInvoiceItemDetails

delete from DbQuotationHeader
delete from DbQuotationHsnSacDetails
delete from DbQuotationItemDetails

delete from DbSalesInvoiceHeader
delete from DbSalesInvoiceHsnSacDetails
delete from DbSalesInvoiceItemDetails

delete from DbSupplier
delete from DbSupplierLedger

end

GO
/****** Object:  Table [dbo].[DbBankAccount]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbBankAccount](
	[BankAccountId] [int] NOT NULL,
	[AccountNo] [varchar](50) NULL,
	[AccountName] [varchar](50) NOT NULL,
	[AccountType] [varchar](50) NULL,
	[BankName] [varchar](50) NULL,
	[Branch] [varchar](50) NULL,
	[Ifsc] [varchar](50) NULL,
	[Balance] [real] NOT NULL,
 CONSTRAINT [PK_DbBankAccount] PRIMARY KEY CLUSTERED 
(
	[BankAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbBankTransaction]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbBankTransaction](
	[BankTransactionId] [int] NOT NULL,
	[BankAccountId] [int] NOT NULL,
	[TransactionDate] [date] NOT NULL,
	[TransactionType] [varchar](50) NOT NULL,
	[Debit] [real] NULL,
	[Credit] [real] NULL,
	[Description] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DbBankTransaction] PRIMARY KEY CLUSTERED 
(
	[BankTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbCategory]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbCategory](
	[CategoryId] [int] NOT NULL,
	[CategoryName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DbCategory] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbCompany]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbCompany](
	[CompanyId] [int] NOT NULL,
	[CompanyName] [varchar](100) NOT NULL,
	[LogoPath] [varchar](max) NULL,
	[Gstin] [varchar](15) NULL,
	[Pan] [varchar](10) NULL,
	[Mobile] [varchar](10) NULL,
	[Landline] [varchar](12) NULL,
	[Email] [varchar](100) NULL,
	[Website] [varchar](100) NULL,
	[Address] [varchar](100) NULL,
	[Pincode] [varchar](6) NULL,
	[City] [varchar](100) NOT NULL,
	[State] [varchar](27) NOT NULL,
	[Password] [varchar](100) NOT NULL,
	[AccountNo] [varchar](100) NULL,
	[AccountName] [varchar](100) NULL,
	[AccountType] [varchar](100) NULL,
	[BankName] [varchar](100) NULL,
	[Ifsc] [varchar](100) NULL,
	[Branch] [varchar](100) NULL,
	[SmtpHost] [varchar](100) NULL,
	[SmtpPort] [int] NULL,
	[EnableSsl] [bit] NOT NULL,
	[SmtpUserName] [varchar](100) NULL,
	[SmtpPassword] [varchar](100) NULL,
	[FromAddress] [varchar](100) NULL,
 CONSTRAINT [PK_DbCompany] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbCountry]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbCountry](
	[CountryCode] [varchar](2) NOT NULL,
	[CountryName] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DbCountry] PRIMARY KEY CLUSTERED 
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbCustomer]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbCustomer](
	[CustomerId] [int] NOT NULL,
	[CustomerName] [varchar](100) NOT NULL,
	[Gstin] [varchar](15) NULL,
	[Pan] [varchar](10) NULL,
	[Mobile] [varchar](50) NULL,
	[Landline] [varchar](50) NULL,
	[Email] [varchar](100) NULL,
	[Address] [varchar](100) NULL,
	[Pincode] [varchar](50) NULL,
	[City] [varchar](100) NOT NULL,
	[State] [varchar](100) NOT NULL,
	[Country] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DbCustomer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbCustomerLedger]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbCustomerLedger](
	[CustomerLedgerId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[InvoiceNo] [int] NOT NULL,
	[Prefix] [varchar](10) NULL,
	[ReceiptNo] [int] NULL,
	[PaymentMethod] [varchar](50) NULL,
	[PaymentDate] [date] NOT NULL,
	[BankAccountId] [int] NULL,
	[Credit] [real] NULL,
	[Debit] [real] NULL,
	[Note] [varchar](50) NULL,
	[BankTransactionId] [int] NULL,
	[Description] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DbCustomerLedger] PRIMARY KEY CLUSTERED 
(
	[CustomerLedgerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbDeposit]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbDeposit](
	[DepositId] [int] NOT NULL,
	[DepositDate] [date] NOT NULL,
	[BankAccountId] [int] NOT NULL,
	[PaymentMethod] [varchar](50) NOT NULL,
	[Purpose] [varchar](50) NOT NULL,
	[Amount] [real] NOT NULL,
	[Note] [varchar](50) NULL,
	[BankTransactionId] [int] NOT NULL,
 CONSTRAINT [PK_DbDeposit] PRIMARY KEY CLUSTERED 
(
	[DepositId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbExpense]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbExpense](
	[ExpenseId] [int] NOT NULL,
	[ExpenseDate] [date] NOT NULL,
	[BankAccountId] [int] NOT NULL,
	[PaymentMethod] [varchar](50) NOT NULL,
	[Purpose] [varchar](50) NOT NULL,
	[Amount] [real] NOT NULL,
	[Note] [varchar](50) NULL,
	[BankTransactionId] [int] NOT NULL,
 CONSTRAINT [PK_DbExpense] PRIMARY KEY CLUSTERED 
(
	[ExpenseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbItem]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbItem](
	[ItemCode] [nvarchar](100) NOT NULL,
	[ItemName] [nvarchar](100) NOT NULL,
	[ItemDescription] [nvarchar](100) NOT NULL,
	[HsnSac] [varchar](10) NOT NULL,
	[TaxRate] [real] NOT NULL,
	[Cess] [real] NOT NULL,
	[PurchasePrice] [real] NOT NULL,
	[PurchasePriceInclusiveTax] [bit] NOT NULL,
	[SalesPrice] [real] NOT NULL,
	[SalesPriceInclusiveTax] [bit] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[UnitId] [int] NOT NULL,
	[StockQuantity] [real] NOT NULL,
	[AlertQuantity] [real] NOT NULL,
 CONSTRAINT [PK_DbItem] PRIMARY KEY CLUSTERED 
(
	[ItemCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbPaymentMethod]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbPaymentMethod](
	[PaymentMethodId] [int] NOT NULL,
	[PaymentMethodName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DbPaymentMethod] PRIMARY KEY CLUSTERED 
(
	[PaymentMethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbPaymentTerm]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbPaymentTerm](
	[PaymentTermId] [int] NOT NULL,
	[PaymentTermName] [varchar](50) NOT NULL,
	[DueDays] [int] NOT NULL,
 CONSTRAINT [PK_DbPaymentTerm] PRIMARY KEY CLUSTERED 
(
	[PaymentTermId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbPurchaseInvoiceHeader]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbPurchaseInvoiceHeader](
	[InvoiceNo] [int] NOT NULL,
	[Prefix] [varchar](10) NOT NULL,
	[InvoiceName] [varchar](5) NOT NULL,
	[InvoiceDate] [date] NOT NULL,
	[SupplierId] [int] NOT NULL,
	[PlaceOfSupply] [varchar](25) NOT NULL,
	[ReverseCharge] [bit] NOT NULL,
	[DiscountOnAll] [bit] NOT NULL,
	[TotalDiscount] [real] NOT NULL,
	[DeliveryNote] [varchar](25) NULL,
	[DeliveryNoteDate] [date] NULL,
	[PaymentMethod] [varchar](25) NOT NULL,
	[PaymentTerm] [varchar](25) NOT NULL,
	[DueDate] [date] NULL,
	[SupplierReference] [varchar](25) NULL,
	[OtherReference] [varchar](25) NULL,
	[PoNo] [varchar](25) NULL,
	[PoDate] [date] NULL,
	[DespatchDocumentNo] [varchar](25) NULL,
	[DespatchedThrough] [varchar](25) NULL,
	[TotalValue] [real] NOT NULL,
	[TotalDiscountRs] [real] NOT NULL,
	[TotalTaxableValue] [real] NOT NULL,
	[TotalCgstAmount] [real] NOT NULL,
	[TotalSgstAmount] [real] NOT NULL,
	[TotalIgstAmount] [real] NOT NULL,
	[TotalTaxValue] [real] NOT NULL,
	[TotalCessRs] [real] NOT NULL,
	[RoundOff] [bit] NOT NULL,
	[RoundOffValue] [real] NOT NULL,
	[Total] [real] NOT NULL,
	[Terms] [varchar](112) NULL,
	[Paid] [real] NOT NULL,
	[Balance] [real] NOT NULL,
	[Status] [varchar](14) NOT NULL,
 CONSTRAINT [PK_DbPurchaseInvoiceHeader] PRIMARY KEY CLUSTERED 
(
	[InvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbPurchaseInvoiceHsnSacDetails]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbPurchaseInvoiceHsnSacDetails](
	[Id] [int] NOT NULL,
	[InvoiceNo] [int] NOT NULL,
	[HsnSac] [varchar](10) NULL,
	[TaxableValue] [real] NOT NULL,
	[CgstRate] [real] NOT NULL,
	[CgstAmount] [real] NOT NULL,
	[SgstRate] [real] NOT NULL,
	[SgstAmount] [real] NOT NULL,
	[IgstRate] [real] NOT NULL,
	[IgstAmount] [real] NOT NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_DbPurchaseInvoiceHsnSacDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[InvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbPurchaseInvoiceItemDetails]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbPurchaseInvoiceItemDetails](
	[Id] [int] NOT NULL,
	[InvoiceNo] [int] NOT NULL,
	[ItemCode] [nvarchar](100) NOT NULL,
	[ItemName] [nvarchar](100) NOT NULL,
	[ItemDescription] [nvarchar](100) NULL,
	[HsnSac] [varchar](10) NULL,
	[Price] [real] NOT NULL,
	[InclusiveTax] [bit] NOT NULL,
	[Quantity] [real] NOT NULL,
	[UnitId] [int] NOT NULL,
	[SubAmount] [real] NOT NULL,
	[Discount] [real] NOT NULL,
	[DiscountRs] [real] NOT NULL,
	[TaxablePrice] [real] NOT NULL,
	[TaxableValue] [real] NOT NULL,
	[TaxRate] [real] NOT NULL,
	[CgstRate] [real] NOT NULL,
	[SgstRate] [real] NOT NULL,
	[IgstRate] [real] NOT NULL,
	[CgstAmount] [real] NOT NULL,
	[SgstAmount] [real] NOT NULL,
	[IgstAmount] [real] NOT NULL,
	[Cess] [real] NOT NULL,
	[CessRs] [real] NOT NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_DbPurchaseInvoiceItemDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[InvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbQuotationHeader]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbQuotationHeader](
	[QuotationNo] [int] NOT NULL,
	[Prefix] [varchar](10) NOT NULL,
	[QuotationName] [varchar](5) NOT NULL,
	[QuotationDate] [date] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[PlaceOfSupply] [varchar](25) NOT NULL,
	[ReverseCharge] [bit] NOT NULL,
	[DiscountOnAll] [bit] NOT NULL,
	[TotalDiscount] [real] NOT NULL,
	[DeliveryNote] [varchar](25) NULL,
	[DeliveryNoteDate] [date] NULL,
	[PaymentMethod] [varchar](25) NOT NULL,
	[PaymentTerm] [varchar](25) NOT NULL,
	[DueDate] [date] NULL,
	[SupplierReference] [varchar](25) NULL,
	[OtherReference] [varchar](25) NULL,
	[PoNo] [varchar](25) NULL,
	[PoDate] [date] NULL,
	[DespatchDocumentNo] [varchar](25) NULL,
	[DespatchedThrough] [varchar](25) NULL,
	[TotalValue] [real] NOT NULL,
	[TotalDiscountRs] [real] NOT NULL,
	[TotalTaxableValue] [real] NOT NULL,
	[TotalCgstAmount] [real] NOT NULL,
	[TotalSgstAmount] [real] NOT NULL,
	[TotalIgstAmount] [real] NOT NULL,
	[TotalTaxValue] [real] NOT NULL,
	[TotalCessRs] [real] NOT NULL,
	[RoundOff] [bit] NOT NULL,
	[RoundOffValue] [real] NOT NULL,
	[Total] [real] NOT NULL,
	[Terms] [varchar](112) NULL,
 CONSTRAINT [PK_DbQuotationHeader] PRIMARY KEY CLUSTERED 
(
	[QuotationNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbQuotationHsnSacDetails]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbQuotationHsnSacDetails](
	[Id] [int] NOT NULL,
	[QuotationNo] [int] NOT NULL,
	[HsnSac] [varchar](10) NULL,
	[TaxableValue] [real] NOT NULL,
	[CgstRate] [real] NOT NULL,
	[CgstAmount] [real] NOT NULL,
	[SgstRate] [real] NOT NULL,
	[SgstAmount] [real] NOT NULL,
	[IgstRate] [real] NOT NULL,
	[IgstAmount] [real] NOT NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_DbQuotationHsnSacDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[QuotationNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbQuotationItemDetails]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbQuotationItemDetails](
	[Id] [int] NOT NULL,
	[QuotationNo] [int] NOT NULL,
	[ItemCode] [nvarchar](100) NOT NULL,
	[ItemName] [nvarchar](100) NOT NULL,
	[ItemDescription] [nvarchar](100) NULL,
	[HsnSac] [varchar](10) NULL,
	[Price] [real] NOT NULL,
	[InclusiveTax] [bit] NOT NULL,
	[Quantity] [real] NOT NULL,
	[UnitId] [int] NOT NULL,
	[SubAmount] [real] NOT NULL,
	[Discount] [real] NOT NULL,
	[DiscountRs] [real] NOT NULL,
	[TaxablePrice] [real] NOT NULL,
	[TaxableValue] [real] NOT NULL,
	[TaxRate] [real] NOT NULL,
	[CgstRate] [real] NOT NULL,
	[SgstRate] [real] NOT NULL,
	[IgstRate] [real] NOT NULL,
	[CgstAmount] [real] NOT NULL,
	[SgstAmount] [real] NOT NULL,
	[IgstAmount] [real] NOT NULL,
	[Cess] [real] NOT NULL,
	[CessRs] [real] NOT NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_DbQuotationItemDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[QuotationNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbSalesInvoiceHeader]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbSalesInvoiceHeader](
	[InvoiceNo] [int] NOT NULL,
	[Prefix] [varchar](10) NOT NULL,
	[InvoiceName] [varchar](5) NOT NULL,
	[InvoiceDate] [date] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[PlaceOfSupply] [varchar](25) NOT NULL,
	[ReverseCharge] [bit] NOT NULL,
	[DiscountOnAll] [bit] NOT NULL,
	[TotalDiscount] [real] NOT NULL,
	[DeliveryNote] [varchar](25) NULL,
	[DeliveryNoteDate] [date] NULL,
	[PaymentMethod] [varchar](25) NOT NULL,
	[PaymentTerm] [varchar](25) NOT NULL,
	[DueDate] [date] NULL,
	[SupplierReference] [varchar](25) NULL,
	[OtherReference] [varchar](25) NULL,
	[PoNo] [varchar](25) NULL,
	[PoDate] [date] NULL,
	[DespatchDocumentNo] [varchar](25) NULL,
	[DespatchedThrough] [varchar](25) NULL,
	[TotalValue] [real] NOT NULL,
	[TotalDiscountRs] [real] NOT NULL,
	[TotalTaxableValue] [real] NOT NULL,
	[TotalCgstAmount] [real] NOT NULL,
	[TotalSgstAmount] [real] NOT NULL,
	[TotalIgstAmount] [real] NOT NULL,
	[TotalTaxValue] [real] NOT NULL,
	[TotalCessRs] [real] NOT NULL,
	[RoundOff] [bit] NOT NULL,
	[RoundOffValue] [real] NOT NULL,
	[Total] [real] NOT NULL,
	[Terms] [varchar](112) NULL,
	[Paid] [real] NOT NULL,
	[Balance] [real] NOT NULL,
	[Status] [varchar](14) NOT NULL,
 CONSTRAINT [PK_DbSalesInvoiceHeader] PRIMARY KEY CLUSTERED 
(
	[InvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbSalesInvoiceHsnSacDetails]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbSalesInvoiceHsnSacDetails](
	[Id] [int] NOT NULL,
	[InvoiceNo] [int] NOT NULL,
	[HsnSac] [varchar](10) NULL,
	[TaxableValue] [real] NOT NULL,
	[CgstRate] [real] NOT NULL,
	[CgstAmount] [real] NOT NULL,
	[SgstRate] [real] NOT NULL,
	[SgstAmount] [real] NOT NULL,
	[IgstRate] [real] NOT NULL,
	[IgstAmount] [real] NOT NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_DbSalesInvoiceHsnSacDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[InvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbSalesInvoiceItemDetails]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbSalesInvoiceItemDetails](
	[Id] [int] NOT NULL,
	[InvoiceNo] [int] NOT NULL,
	[ItemCode] [nvarchar](100) NOT NULL,
	[ItemName] [nvarchar](100) NOT NULL,
	[ItemDescription] [nvarchar](100) NULL,
	[HsnSac] [varchar](10) NULL,
	[Price] [real] NOT NULL,
	[InclusiveTax] [bit] NOT NULL,
	[Quantity] [real] NOT NULL,
	[UnitId] [int] NOT NULL,
	[SubAmount] [real] NOT NULL,
	[Discount] [real] NOT NULL,
	[DiscountRs] [real] NOT NULL,
	[TaxablePrice] [real] NOT NULL,
	[TaxableValue] [real] NOT NULL,
	[TaxRate] [real] NOT NULL,
	[CgstRate] [real] NOT NULL,
	[SgstRate] [real] NOT NULL,
	[IgstRate] [real] NOT NULL,
	[CgstAmount] [real] NOT NULL,
	[SgstAmount] [real] NOT NULL,
	[IgstAmount] [real] NOT NULL,
	[Cess] [real] NOT NULL,
	[CessRs] [real] NOT NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_DbSalesInvoiceItemDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[InvoiceNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbState]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbState](
	[StateId] [int] NOT NULL,
	[StateName] [varchar](25) NOT NULL,
 CONSTRAINT [PK_DbState_1] PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbStateCode]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbStateCode](
	[StateCode] [varchar](2) NOT NULL,
	[StateName] [varchar](25) NOT NULL,
 CONSTRAINT [PK_DbState] PRIMARY KEY CLUSTERED 
(
	[StateCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbSupplier]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbSupplier](
	[SupplierId] [int] NOT NULL,
	[SupplierName] [varchar](100) NOT NULL,
	[Gstin] [varchar](15) NULL,
	[Pan] [varchar](10) NULL,
	[Mobile] [varchar](50) NULL,
	[Landline] [varchar](50) NULL,
	[Email] [varchar](100) NULL,
	[Address] [varchar](100) NULL,
	[Pincode] [varchar](50) NULL,
	[City] [varchar](100) NOT NULL,
	[State] [varchar](100) NOT NULL,
	[Country] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DbSupplier] PRIMARY KEY CLUSTERED 
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbSupplierLedger]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbSupplierLedger](
	[SupplierLedgerId] [int] NOT NULL,
	[SupplierId] [int] NOT NULL,
	[InvoiceNo] [int] NOT NULL,
	[Prefix] [varchar](10) NULL,
	[PaymentNo] [int] NULL,
	[PaymentMethod] [varchar](50) NULL,
	[PaymentDate] [date] NOT NULL,
	[BankAccountId] [int] NULL,
	[Credit] [real] NULL,
	[Debit] [real] NULL,
	[Note] [varchar](50) NULL,
	[BankTransactionId] [int] NULL,
	[Description] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DbSupplierLedger] PRIMARY KEY CLUSTERED 
(
	[SupplierLedgerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbTax]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbTax](
	[TaxId] [int] NOT NULL,
	[TaxName] [varchar](5) NOT NULL,
	[TaxRate] [real] NOT NULL,
 CONSTRAINT [PK_DbTax] PRIMARY KEY CLUSTERED 
(
	[TaxId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DbUnit]    Script Date: 8/1/2019 4:09:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DbUnit](
	[UnitId] [int] NOT NULL,
	[UnitName] [varchar](16) NOT NULL,
 CONSTRAINT [PK_DbUnit] PRIMARY KEY CLUSTERED 
(
	[UnitId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
INSERT [dbo].[DbBankAccount] ([BankAccountId], [AccountNo], [AccountName], [AccountType], [BankName], [Branch], [Ifsc], [Balance]) VALUES (1, N'', N'Cash In Hand', N'', N'', N'', N'', 4500)
GO
INSERT [dbo].[DbBankAccount] ([BankAccountId], [AccountNo], [AccountName], [AccountType], [BankName], [Branch], [Ifsc], [Balance]) VALUES (2, N'915010007266812', N'Axis Bank', N'Savings', N'Axis Bank', N'Gandhipuram', N'UTIB0000090', 25494)
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (5, 1, CAST(0x903F0B00 AS Date), N'Sales', NULL, 2500, N'Received Payment (RC-1) for Invoice INV1')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (6, 1, CAST(0x903F0B00 AS Date), N'Sales', NULL, 2500, N'Received Payment (RC-2) for Invoice INV2')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (7, 2, CAST(0x913F0B00 AS Date), N'Sales', NULL, 2500, N'Received Payment (RC-3) for Invoice INV5')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (22, 2, CAST(0x963F0B00 AS Date), N'Sales', NULL, 2500, N'Received Payment (RC-4) for Invoice INV4')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (23, 1, CAST(0xB03F0B00 AS Date), N'Sales', NULL, 4500, N'Received Payment (RC-5) for Invoice INV6')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (24, 2, CAST(0xAA3F0B00 AS Date), N'Sales', NULL, 10000, N'Received Payment (RC-6) for Invoice INV7')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (25, 2, CAST(0xCA3F0B00 AS Date), N'Sales', NULL, 13000, N'Received Payment (RC-7) for Invoice INV8')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (26, 2, CAST(0xCE3F0B00 AS Date), N'Sales', NULL, 1000, N'Received Payment (RC-8) for Invoice INV9')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (27, 2, CAST(0xAA3F0B00 AS Date), N'Sales', NULL, 500, N'Received Payment (RC-9) for Invoice INV5')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (28, 2, CAST(0xDC3F0B00 AS Date), N'Sales', NULL, 2000, N'Received Payment (RC-10) for Invoice INV3')
GO
INSERT [dbo].[DbBankTransaction] ([BankTransactionId], [BankAccountId], [TransactionDate], [TransactionType], [Debit], [Credit], [Description]) VALUES (29, 2, CAST(0xE63F0B00 AS Date), N'Sales', NULL, 2000, N'Received Payment (RC-11) for Invoice INV2')
GO
INSERT [dbo].[DbCategory] ([CategoryId], [CategoryName]) VALUES (2, N'Software')
GO
INSERT [dbo].[DbCompany] ([CompanyId], [CompanyName], [LogoPath], [Gstin], [Pan], [Mobile], [Landline], [Email], [Website], [Address], [Pincode], [City], [State], [Password], [AccountNo], [AccountName], [AccountType], [BankName], [Ifsc], [Branch], [SmtpHost], [SmtpPort], [EnableSsl], [SmtpUserName], [SmtpPassword], [FromAddress]) VALUES (1, N'Code Builders', N'../Logo/Logo.png', N'xxxxxxxxxxxxxxx', N'', N'8675675143', N'', N'mail.csbalaji@gmail.com', N'codebuilders.in', N'Site No. 40, Krishna Garden, Uppilipalayam', N'641015', N'Coimbatore', N'Tamil Nadu', N'6633', N'915010007266812', N'', N'', N'Axis Bank', N'UTIB0000090', N'Coimbatore', N'smtp.gmail.com', 587, 1, N'csbalaji143@gmail.com', N'Ph@8675675143', N'csbalaji143@gmail.com')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AD', N'AndorrA')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AE', N'United Arab Emirates')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AF', N'Afghanistan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AG', N'Antigua and Barbuda')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AI', N'Anguilla')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AL', N'Albania')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AM', N'Armenia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AN', N'Netherlands Antilles')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AO', N'Angola')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AQ', N'Antarctica')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AR', N'Argentina')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AS', N'American Samoa')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AT', N'Austria')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AU', N'Australia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AW', N'Aruba')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AX', N'Åland Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'AZ', N'Azerbaijan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BA', N'Bosnia and Herzegovina')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BB', N'Barbados')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BD', N'Bangladesh')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BE', N'Belgium')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BF', N'Burkina Faso')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BG', N'Bulgaria')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BH', N'Bahrain')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BI', N'Burundi')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BJ', N'Benin')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BM', N'Bermuda')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BN', N'Brunei Darussalam')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BO', N'Bolivia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BR', N'Brazil')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BS', N'Bahamas')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BT', N'Bhutan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BV', N'Bouvet Island')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BW', N'Botswana')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BY', N'Belarus')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'BZ', N'Belize')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CA', N'Canada')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CC', N'Cocos (Keeling) Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CD', N'Congo, The Democratic Republic of the')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CF', N'Central African Republic')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CG', N'Congo')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CH', N'Switzerland')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CI', N'Cote D''Ivoire')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CK', N'Cook Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CL', N'Chile')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CM', N'Cameroon')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CN', N'China')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CO', N'Colombia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CR', N'Costa Rica')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CS', N'Serbia and Montenegro')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CU', N'Cuba')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CV', N'Cape Verde')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CX', N'Christmas Island')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CY', N'Cyprus')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'CZ', N'Czech Republic')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'DE', N'Germany')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'DJ', N'Djibouti')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'DK', N'Denmark')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'DM', N'Dominica')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'DO', N'Dominican Republic')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'DZ', N'Algeria')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'EC', N'Ecuador')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'EE', N'Estonia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'EG', N'Egypt')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'EH', N'Western Sahara')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ER', N'Eritrea')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ES', N'Spain')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ET', N'Ethiopia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'FI', N'Finland')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'FJ', N'Fiji')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'FK', N'Falkland Islands (Malvinas)')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'FM', N'Micronesia, Federated States of')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'FO', N'Faroe Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'FR', N'France')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GA', N'Gabon')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GB', N'United Kingdom')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GD', N'Grenada')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GE', N'Georgia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GF', N'French Guiana')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GG', N'Guernsey')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GH', N'Ghana')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GI', N'Gibraltar')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GL', N'Greenland')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GM', N'Gambia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GN', N'Guinea')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GP', N'Guadeloupe')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GQ', N'Equatorial Guinea')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GR', N'Greece')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GS', N'South Georgia and the South Sandwich Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GT', N'Guatemala')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GU', N'Guam')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GW', N'Guinea-Bissau')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'GY', N'Guyana')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'HK', N'Hong Kong')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'HM', N'Heard Island and Mcdonald Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'HN', N'Honduras')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'HR', N'Croatia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'HT', N'Haiti')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'HU', N'Hungary')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ID', N'Indonesia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IE', N'Ireland')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IL', N'Israel')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IM', N'Isle of Man')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IN', N'India')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IO', N'British Indian Ocean Territory')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IQ', N'Iraq')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IR', N'Iran, Islamic Republic Of')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IS', N'Iceland')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'IT', N'Italy')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'JE', N'Jersey')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'JM', N'Jamaica')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'JO', N'Jordan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'JP', N'Japan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KE', N'Kenya')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KG', N'Kyrgyzstan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KH', N'Cambodia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KI', N'Kiribati')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KM', N'Comoros')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KN', N'Saint Kitts and Nevis')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KP', N'Korea, Democratic People''S Republic of')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KR', N'Korea, Republic of')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KW', N'Kuwait')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KY', N'Cayman Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'KZ', N'Kazakhstan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LA', N'Lao People''S Democratic Republic')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LB', N'Lebanon')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LC', N'Saint Lucia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LI', N'Liechtenstein')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LK', N'Sri Lanka')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LR', N'Liberia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LS', N'Lesotho')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LT', N'Lithuania')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LU', N'Luxembourg')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LV', N'Latvia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'LY', N'Libyan Arab Jamahiriya')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MA', N'Morocco')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MC', N'Monaco')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MD', N'Moldova, Republic of')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MG', N'Madagascar')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MH', N'Marshall Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MK', N'Macedonia, The Former Yugoslav Republic of')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ML', N'Mali')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MM', N'Myanmar')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MN', N'Mongolia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MO', N'Macao')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MP', N'Northern Mariana Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MQ', N'Martinique')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MR', N'Mauritania')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MS', N'Montserrat')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MT', N'Malta')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MU', N'Mauritius')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MV', N'Maldives')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MW', N'Malawi')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MX', N'Mexico')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MY', N'Malaysia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'MZ', N'Mozambique')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NA', N'Namibia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NC', N'New Caledonia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NE', N'Niger')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NF', N'Norfolk Island')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NG', N'Nigeria')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NI', N'Nicaragua')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NL', N'Netherlands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NO', N'Norway')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NP', N'Nepal')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NR', N'Nauru')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NU', N'Niue')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'NZ', N'New Zealand')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'OM', N'Oman')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PA', N'Panama')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PE', N'Peru')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PF', N'French Polynesia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PG', N'Papua New Guinea')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PH', N'Philippines')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PK', N'Pakistan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PL', N'Poland')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PM', N'Saint Pierre and Miquelon')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PN', N'Pitcairn')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PR', N'Puerto Rico')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PS', N'Palestinian Territory, Occupied')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PT', N'Portugal')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PW', N'Palau')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'PY', N'Paraguay')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'QA', N'Qatar')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'RE', N'Reunion')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'RO', N'Romania')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'RU', N'Russian Federation')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'RW', N'RWANDA')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SA', N'Saudi Arabia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SB', N'Solomon Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SC', N'Seychelles')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SD', N'Sudan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SE', N'Sweden')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SG', N'Singapore')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SH', N'Saint Helena')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SI', N'Slovenia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SJ', N'Svalbard and Jan Mayen')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SK', N'Slovakia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SL', N'Sierra Leone')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SM', N'San Marino')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SN', N'Senegal')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SO', N'Somalia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SR', N'Suriname')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ST', N'Sao Tome and Principe')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SV', N'El Salvador')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SY', N'Syrian Arab Republic')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'SZ', N'Swaziland')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TC', N'Turks and Caicos Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TD', N'Chad')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TF', N'French Southern Territories')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TG', N'Togo')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TH', N'Thailand')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TJ', N'Tajikistan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TK', N'Tokelau')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TL', N'Timor-Leste')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TM', N'Turkmenistan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TN', N'Tunisia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TO', N'Tonga')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TR', N'Turkey')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TT', N'Trinidad and Tobago')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TV', N'Tuvalu')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TW', N'Taiwan, Province of China')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'TZ', N'Tanzania, United Republic of')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'UA', N'Ukraine')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'UG', N'Uganda')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'UM', N'United States Minor Outlying Islands')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'US', N'United States')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'UY', N'Uruguay')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'UZ', N'Uzbekistan')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'VA', N'Holy See (Vatican City State)')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'VC', N'Saint Vincent and the Grenadines')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'VE', N'Venezuela')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'VG', N'Virgin Islands, British')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'VI', N'Virgin Islands, U.S.')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'VN', N'Viet Nam')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'VU', N'Vanuatu')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'WF', N'Wallis and Futuna')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'WS', N'Samoa')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'YE', N'Yemen')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'YT', N'Mayotte')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ZA', N'South Africa')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ZM', N'Zambia')
GO
INSERT [dbo].[DbCountry] ([CountryCode], [CountryName]) VALUES (N'ZW', N'Zimbabwe')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (1, N'A V R Industries', N'33AGZPA3553H1ZV', N'AGZPA3553H', N'8778040910', N'9345026626', N'avrindustries@gmail.com', N'S. F. No. 282 / 1-A, Site No. 1, Kumaran Layout, Subbanaickenpudur, Chiinavedampatty, Ganapathy', N'641006', N'Coimbatore', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (2, N'SMT', N'', N'', N'7373819161', N'', N'', N'', N'', N'Coimbatore', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (3, N'SSK Handlooms', N'33BHUPS0296N1ZG', N'BHUPS0296N', N'9894115875', N'8124709754', N'sskhandloomscbe@gmail.com', N'106,Murugan Nagar,1St Cross,V.K.Road,Peelamedu', N'641004', N'Coimbatore', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (4, N'Annai Naturals', N'33BRMPK6931Q1Z3', N'BRMPK6931Q', N'9360247788', N'04362240788', N'herbravi@gmail.com', N'G5,JEYANTH COMPLEX,M.C.ROAD', N'613004', N'THANJAVUR', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (5, N'MODI INFONET DIGITAL NETWORK', N'27HUOPK4454B1ZF', N'HUOPK445AB', N'9422116679', N'8149049849', N'accounts@myinfonet.in', N' BLOCK NO 7, SHUBHAN HOTEL MANGAL KARYALAY BUILDING', N'445304', N'Wani', N'Maharashtra', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (6, N'Balaji', N'', N'', N'', N'', N'mail.csbalaji@gmail.com', N'', N'', N'', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (7, N'RP TRADERS', N'33BMHPA9621DIZI', N'', N'9940919938', N'', N'arunkumar.s.5051@gmail.com', N'No.13-B/2, KK Nagar, Police Station Opposite Road', N'641023', N'Coimbatore', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (8, N'Grace n Bakes', N'', N'', N'', N'', N'', N'', N'', N'', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (9, N'Shivam Technology Solutions', N'', N'', N'9028121378', N'', N'info@shivamtechno.com', N'', N'', N'', N'Maharashtra', N'India')
GO
INSERT [dbo].[DbCustomer] ([CustomerId], [CustomerName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (10, N'Techswing', N'', N'', N'', N'', N'', N'', N'', N'', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (1, 3, 1, NULL, NULL, NULL, CAST(0x903F0B00 AS Date), NULL, NULL, 3500, NULL, NULL, N'Invoice #INV1')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (2, 3, 1, N'RC-', 1, N'Cash', CAST(0x903F0B00 AS Date), 1, 2500, NULL, N'', 5, N'Payment (RC-1) to Invoice INV1')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (3, 4, 2, NULL, NULL, NULL, CAST(0x903F0B00 AS Date), NULL, NULL, 4500, NULL, NULL, N'Invoice #INV2')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (4, 2, 3, NULL, NULL, NULL, CAST(0x903F0B00 AS Date), NULL, NULL, 2000, NULL, NULL, N'Invoice #INV3')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (5, 1, 4, NULL, NULL, NULL, CAST(0x903F0B00 AS Date), NULL, NULL, 2500, NULL, NULL, N'Invoice #INV4')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (6, 5, 5, NULL, NULL, NULL, CAST(0x903F0B00 AS Date), NULL, NULL, 3000, NULL, NULL, N'Invoice #INV5')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (7, 4, 2, N'RC-', 2, N'Cash', CAST(0x903F0B00 AS Date), 1, 2500, NULL, N'', 6, N'Payment (RC-2) to Invoice INV2')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (8, 5, 5, N'RC-', 3, N'Bank Transfer', CAST(0x913F0B00 AS Date), 2, 2500, NULL, N'', 7, N'Payment (RC-3) to Invoice INV5')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (9, 1, 4, N'RC-', 4, N'Bank Transfer', CAST(0x963F0B00 AS Date), 2, 2500, NULL, N'', 22, N'Payment (RC-4) to Invoice INV4')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (10, 7, 6, NULL, NULL, NULL, CAST(0xB03F0B00 AS Date), NULL, NULL, 5000, NULL, NULL, N'Invoice #INV6')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (11, 7, 6, N'RC-', 5, N'Cash', CAST(0xB03F0B00 AS Date), 1, 4500, NULL, N'', 23, N'Payment (RC-5) to Invoice INV6')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (12, 8, 7, NULL, NULL, NULL, CAST(0xAA3F0B00 AS Date), NULL, NULL, 10000, NULL, NULL, N'Invoice #INV7')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (13, 8, 7, N'RC-', 6, N'Cheque', CAST(0xAA3F0B00 AS Date), 2, 10000, NULL, N'', 24, N'Payment (RC-6) to Invoice INV7')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (14, 10, 8, NULL, NULL, NULL, CAST(0xCA3F0B00 AS Date), NULL, NULL, 13000, NULL, NULL, N'Invoice #INV8')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (15, 9, 9, NULL, NULL, NULL, CAST(0xCF3F0B00 AS Date), NULL, NULL, 1000, NULL, NULL, N'Invoice #INV9')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (16, 10, 8, N'RC-', 7, N'Bank Transfer', CAST(0xCA3F0B00 AS Date), 2, 13000, NULL, N'', 25, N'Payment (RC-7) to Invoice INV8')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (17, 9, 9, N'RC-', 8, N'Bank Transfer', CAST(0xCE3F0B00 AS Date), 2, 1000, NULL, N'', 26, N'Payment (RC-8) to Invoice INV9')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (18, 5, 5, N'RC-', 9, N'Bank Transfer', CAST(0xAA3F0B00 AS Date), 2, 500, NULL, N'', 27, N'Payment (RC-9) to Invoice INV5')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (19, 2, 3, N'RC-', 10, N'Cash', CAST(0xDC3F0B00 AS Date), 2, 2000, NULL, N'', 28, N'Payment (RC-10) to Invoice INV3')
GO
INSERT [dbo].[DbCustomerLedger] ([CustomerLedgerId], [CustomerId], [InvoiceNo], [Prefix], [ReceiptNo], [PaymentMethod], [PaymentDate], [BankAccountId], [Credit], [Debit], [Note], [BankTransactionId], [Description]) VALUES (20, 4, 2, N'RC-', 11, N'Cash', CAST(0xE63F0B00 AS Date), 2, 2000, NULL, N'', 29, N'Payment (RC-11) to Invoice INV2')
GO
INSERT [dbo].[DbItem] ([ItemCode], [ItemName], [ItemDescription], [HsnSac], [TaxRate], [Cess], [PurchasePrice], [PurchasePriceInclusiveTax], [SalesPrice], [SalesPriceInclusiveTax], [CategoryId], [UnitId], [StockQuantity], [AlertQuantity]) VALUES (N'1', N'Billing Software', N'', N'', 18, 0, 0, 0, 0, 0, 2, 46, -2, 0)
GO
INSERT [dbo].[DbItem] ([ItemCode], [ItemName], [ItemDescription], [HsnSac], [TaxRate], [Cess], [PurchasePrice], [PurchasePriceInclusiveTax], [SalesPrice], [SalesPriceInclusiveTax], [CategoryId], [UnitId], [StockQuantity], [AlertQuantity]) VALUES (N'2', N'AMC', N'', N'', 18, 0, 0, 0, 0, 0, 2, 1, -1, 0)
GO
INSERT [dbo].[DbItem] ([ItemCode], [ItemName], [ItemDescription], [HsnSac], [TaxRate], [Cess], [PurchasePrice], [PurchasePriceInclusiveTax], [SalesPrice], [SalesPriceInclusiveTax], [CategoryId], [UnitId], [StockQuantity], [AlertQuantity]) VALUES (N'3', N'Odometer Sensor Reader', N'', N'', 18, 0, 0, 0, 0, 0, 2, 46, -1, 0)
GO
INSERT [dbo].[DbItem] ([ItemCode], [ItemName], [ItemDescription], [HsnSac], [TaxRate], [Cess], [PurchasePrice], [PurchasePriceInclusiveTax], [SalesPrice], [SalesPriceInclusiveTax], [CategoryId], [UnitId], [StockQuantity], [AlertQuantity]) VALUES (N'4', N'SGI Customization', N'', N'', 18, 0, 0, 0, 0, 0, 2, 1, 0, 0)
GO
INSERT [dbo].[DbPaymentMethod] ([PaymentMethodId], [PaymentMethodName]) VALUES (1, N'Cash')
GO
INSERT [dbo].[DbPaymentMethod] ([PaymentMethodId], [PaymentMethodName]) VALUES (2, N'Cheque')
GO
INSERT [dbo].[DbPaymentMethod] ([PaymentMethodId], [PaymentMethodName]) VALUES (3, N'Credit')
GO
INSERT [dbo].[DbPaymentMethod] ([PaymentMethodId], [PaymentMethodName]) VALUES (4, N'Bank Transfer')
GO
INSERT [dbo].[DbPaymentMethod] ([PaymentMethodId], [PaymentMethodName]) VALUES (5, N'Mobile Banking')
GO
INSERT [dbo].[DbPaymentMethod] ([PaymentMethodId], [PaymentMethodName]) VALUES (6, N'Mobile Money')
GO
INSERT [dbo].[DbPaymentMethod] ([PaymentMethodId], [PaymentMethodName]) VALUES (7, N'Paypal')
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (1, N'Due 7', 7)
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (2, N'Due 10', 10)
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (3, N'Due 15', 15)
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (4, N'Due 30', 30)
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (5, N'Due 45', 45)
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (6, N'Due 60', 60)
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (7, N'Due 90', 90)
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (8, N'Due on Receipt', 0)
GO
INSERT [dbo].[DbPaymentTerm] ([PaymentTermId], [PaymentTermName], [DueDays]) VALUES (9, N'Due on the specified date', -1)
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (1, N'INV', N'1', CAST(0x903F0B00 AS Date), 3, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0x903F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 3500, 0, 3500, 0, 0, 0, 0, 0, 0, 0, 3500, NULL, 2500, 1000, N'Partially')
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (2, N'INV', N'2', CAST(0x903F0B00 AS Date), 4, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0x903F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 4500, 0, 4500, 0, 0, 0, 0, 0, 0, 0, 4500, NULL, 4500, 0, N'Paid')
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (3, N'INV', N'3', CAST(0x903F0B00 AS Date), 2, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0x903F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 2000, 0, 2000, 0, 0, 0, 0, 0, 0, 0, 2000, NULL, 2000, 0, N'Paid')
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (4, N'INV', N'4', CAST(0x903F0B00 AS Date), 1, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0x903F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 2500, 0, 2500, 0, 0, 0, 0, 0, 0, 0, 2500, NULL, 2500, 0, N'Paid')
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (5, N'INV', N'5', CAST(0x903F0B00 AS Date), 5, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0x903F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 3000, 0, 3000, 0, 0, 0, 0, 0, 0, 0, 3000, NULL, 3000, 0, N'Paid')
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (6, N'INV', N'6', CAST(0xB03F0B00 AS Date), 7, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0xB03F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 5000, 0, 5000, 0, 0, 0, 0, 0, 0, 0, 5000, NULL, 4500, 500, N'Partially')
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (7, N'INV', N'7', CAST(0xAA3F0B00 AS Date), 8, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0xAA3F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 10000, 0, 10000, 0, 0, 0, 0, 0, 0, 0, 10000, NULL, 10000, 0, N'Paid')
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (8, N'INV', N'8', CAST(0xCA3F0B00 AS Date), 10, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0xCA3F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 13000, 0, 13000, 0, 0, 0, 0, 0, 0, 0, 13000, NULL, 13000, 0, N'Paid')
GO
INSERT [dbo].[DbSalesInvoiceHeader] ([InvoiceNo], [Prefix], [InvoiceName], [InvoiceDate], [CustomerId], [PlaceOfSupply], [ReverseCharge], [DiscountOnAll], [TotalDiscount], [DeliveryNote], [DeliveryNoteDate], [PaymentMethod], [PaymentTerm], [DueDate], [SupplierReference], [OtherReference], [PoNo], [PoDate], [DespatchDocumentNo], [DespatchedThrough], [TotalValue], [TotalDiscountRs], [TotalTaxableValue], [TotalCgstAmount], [TotalSgstAmount], [TotalIgstAmount], [TotalTaxValue], [TotalCessRs], [RoundOff], [RoundOffValue], [Total], [Terms], [Paid], [Balance], [Status]) VALUES (9, N'INV', N'9', CAST(0xCF3F0B00 AS Date), 9, N'Tamil Nadu', 0, 0, 0, NULL, NULL, N'Cash', N'Due on Receipt', CAST(0xCF3F0B00 AS Date), NULL, NULL, NULL, NULL, NULL, NULL, 1000, 0, 1000, 0, 0, 0, 0, 0, 0, 0, 1000, NULL, 1000, 0, N'Paid')
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 1, NULL, 3500, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 2, NULL, 4500, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 3, NULL, 2000, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 4, NULL, 2500, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 5, NULL, 3000, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 6, NULL, 5000, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 7, NULL, 10000, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 8, NULL, 13000, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceHsnSacDetails] ([Id], [InvoiceNo], [HsnSac], [TaxableValue], [CgstRate], [CgstAmount], [SgstRate], [SgstAmount], [IgstRate], [IgstAmount], [Amount]) VALUES (1, 9, NULL, 1000, 0, 0, 0, 0, 0, 0, 0)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 1, N'1', N'Billing Software', NULL, NULL, 3500, 0, 1, 27, 3500, 0, 0, 3500, 3500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3500)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 2, N'1', N'Billing Software', NULL, NULL, 4500, 0, 1, 27, 4500, 0, 0, 4500, 4500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4500)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 3, N'1', N'Billing Software', NULL, NULL, 2000, 0, 1, 46, 2000, 0, 0, 2000, 2000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2000)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 4, N'1', N'Billing Software', NULL, NULL, 2500, 0, 1, 27, 2500, 0, 0, 2500, 2500, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2500)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 5, N'1', N'Billing Software', NULL, NULL, 1500, 0, 2, 27, 3000, 0, 0, 1500, 3000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3000)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 6, N'1', N'Billing Software', NULL, NULL, 5000, 0, 1, 46, 5000, 0, 0, 5000, 5000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5000)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 7, N'2', N'AMC', NULL, NULL, 10000, 0, 1, 28, 10000, 0, 0, 10000, 10000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 10000)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 8, N'3', N'Odometer Sensor Reader', NULL, NULL, 13000, 0, 1, 46, 13000, 0, 0, 13000, 13000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13000)
GO
INSERT [dbo].[DbSalesInvoiceItemDetails] ([Id], [InvoiceNo], [ItemCode], [ItemName], [ItemDescription], [HsnSac], [Price], [InclusiveTax], [Quantity], [UnitId], [SubAmount], [Discount], [DiscountRs], [TaxablePrice], [TaxableValue], [TaxRate], [CgstRate], [SgstRate], [IgstRate], [CgstAmount], [SgstAmount], [IgstAmount], [Cess], [CessRs], [Amount]) VALUES (1, 9, N'1', N'Billing Software', NULL, NULL, 1000, 0, 1, 46, 1000, 0, 0, 1000, 1000, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1000)
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (1, N'Jammu & Kashmir')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (2, N'Himachal Pradesh')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (3, N'Punjab')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (4, N'Chandigarh')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (5, N'Uttarakhand')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (6, N'Haryana')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (7, N'Delhi')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (8, N'Rajasthan')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (9, N'Uttar Pradesh')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (10, N'Bihar')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (11, N'Sikkim')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (12, N'Arunachal Pradesh')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (13, N'Nagaland')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (14, N'Manipur')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (15, N'Mizoram')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (16, N'Tripura')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (17, N'Meghalaya')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (18, N'Assam')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (19, N'West Bengal')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (20, N'Jharkhand')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (21, N'Orissa')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (22, N'Chhattisgarh')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (23, N'Madhya Pradesh')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (24, N'Gujarat')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (25, N'Daman & Diu')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (26, N'Dadra & Nagar Haveli')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (27, N'Maharashtra')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (28, N'Andhra Pradesh')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (29, N'Karnataka')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (30, N'Goa')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (31, N'Lakshadweep')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (32, N'Kerala')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (33, N'Tamil Nadu')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (34, N'Puducherry')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (35, N'Andaman & Nicobar Islands')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (36, N'Telengana')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (37, N'Andrapradesh(New)')
GO
INSERT [dbo].[DbState] ([StateId], [StateName]) VALUES (38, N'Others')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'01', N'Jammu & Kashmir')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'02', N'Himachal Pradesh')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'03', N'Punjab')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'04', N'Chandigarh')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'05', N'Uttarakhand')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'06', N'Haryana')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'07', N'Delhi')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'08', N'Rajasthan')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'09', N'Uttar Pradesh')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'10', N'Bihar')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'11', N'Sikkim')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'12', N'Arunachal Pradesh')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'13', N'Nagaland')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'14', N'Manipur')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'15', N'Mizoram')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'16', N'Tripura')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'17', N'Meghalaya')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'18', N'Assam')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'19', N'West Bengal')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'20', N'Jharkhand')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'21', N'Orissa')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'22', N'Chhattisgarh')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'23', N'Madhya Pradesh')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'24', N'Gujarat')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'25', N'Daman & Diu')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'26', N'Dadra & Nagar Haveli')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'27', N'Maharashtra')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'28', N'Andhra Pradesh')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'29', N'Karnataka')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'30', N'Goa')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'31', N'Lakshadweep')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'32', N'Kerala')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'33', N'Tamil Nadu')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'34', N'Puducherry')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'35', N'Andaman & Nicobar Islands')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'36', N'Telengana')
GO
INSERT [dbo].[DbStateCode] ([StateCode], [StateName]) VALUES (N'37', N'Andrapradesh(New)')
GO
INSERT [dbo].[DbSupplier] ([SupplierId], [SupplierName], [Gstin], [Pan], [Mobile], [Landline], [Email], [Address], [Pincode], [City], [State], [Country]) VALUES (1, N'Default', N'', N'', N'', N'', N'', N'', N'', N'', N'Tamil Nadu', N'India')
GO
INSERT [dbo].[DbTax] ([TaxId], [TaxName], [TaxRate]) VALUES (1, N'0%', 0)
GO
INSERT [dbo].[DbTax] ([TaxId], [TaxName], [TaxRate]) VALUES (2, N'0.25%', 0.25)
GO
INSERT [dbo].[DbTax] ([TaxId], [TaxName], [TaxRate]) VALUES (3, N'3%', 3)
GO
INSERT [dbo].[DbTax] ([TaxId], [TaxName], [TaxRate]) VALUES (4, N'5%', 5)
GO
INSERT [dbo].[DbTax] ([TaxId], [TaxName], [TaxRate]) VALUES (5, N'12%', 12)
GO
INSERT [dbo].[DbTax] ([TaxId], [TaxName], [TaxRate]) VALUES (6, N'18%', 18)
GO
INSERT [dbo].[DbTax] ([TaxId], [TaxName], [TaxRate]) VALUES (7, N'28%', 28)
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (1, N'')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (2, N'BOU')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (3, N'Bags')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (4, N'Bale')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (5, N'Bottles')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (6, N'Boxes')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (7, N'Buckles')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (8, N'Bunches')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (9, N'Bundles')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (10, N'Cans')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (11, N'Cartons')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (12, N'Centimeter')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (13, N'Cubic Centimeter')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (14, N'Cubic Meter')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (15, N'Dozen')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (16, N'Drums')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (17, N'Grams')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (18, N'Great Gross')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (19, N'Gross')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (20, N'Gross Yards')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (21, N'Kilograms')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (22, N'Kiloliter')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (23, N'Kilometers')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (24, N'Meter')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (25, N'Metric Ton')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (26, N'Milliliters')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (27, N'Numbers')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (28, N'Others')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (29, N'Packs')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (30, N'Pairs')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (31, N'Pieces')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (32, N'Quintal')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (33, N'Rolls')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (34, N'Sets')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (35, N'Square Feet')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (36, N'Square Meter')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (37, N'Square Yards')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (38, N'Tablets')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (39, N'Ten Grams')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (40, N'Thousands')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (41, N'Tonnes')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (42, N'Tubes')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (43, N'US Gallons')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (44, N'Units')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (45, N'Yards')
GO
INSERT [dbo].[DbUnit] ([UnitId], [UnitName]) VALUES (46, N'License')
GO
ALTER TABLE [dbo].[DbBankTransaction]  WITH CHECK ADD  CONSTRAINT [FK_DbBankTransaction_DbBankAccount] FOREIGN KEY([BankAccountId])
REFERENCES [dbo].[DbBankAccount] ([BankAccountId])
GO
ALTER TABLE [dbo].[DbBankTransaction] CHECK CONSTRAINT [FK_DbBankTransaction_DbBankAccount]
GO
ALTER TABLE [dbo].[DbCustomerLedger]  WITH CHECK ADD  CONSTRAINT [FK_DbCustomerLedger_DbBankAccount] FOREIGN KEY([BankAccountId])
REFERENCES [dbo].[DbBankAccount] ([BankAccountId])
GO
ALTER TABLE [dbo].[DbCustomerLedger] CHECK CONSTRAINT [FK_DbCustomerLedger_DbBankAccount]
GO
ALTER TABLE [dbo].[DbCustomerLedger]  WITH CHECK ADD  CONSTRAINT [FK_DbCustomerLedger_DbBankTransaction] FOREIGN KEY([BankTransactionId])
REFERENCES [dbo].[DbBankTransaction] ([BankTransactionId])
GO
ALTER TABLE [dbo].[DbCustomerLedger] CHECK CONSTRAINT [FK_DbCustomerLedger_DbBankTransaction]
GO
ALTER TABLE [dbo].[DbCustomerLedger]  WITH CHECK ADD  CONSTRAINT [FK_DbCustomerLedger_DbCustomer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[DbCustomer] ([CustomerId])
GO
ALTER TABLE [dbo].[DbCustomerLedger] CHECK CONSTRAINT [FK_DbCustomerLedger_DbCustomer]
GO
ALTER TABLE [dbo].[DbCustomerLedger]  WITH CHECK ADD  CONSTRAINT [FK_DbCustomerLedger_DbSalesInvoiceHeader] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[DbCustomer] ([CustomerId])
GO
ALTER TABLE [dbo].[DbCustomerLedger] CHECK CONSTRAINT [FK_DbCustomerLedger_DbSalesInvoiceHeader]
GO
ALTER TABLE [dbo].[DbDeposit]  WITH CHECK ADD  CONSTRAINT [FK_DbDeposit_DbBankAccount] FOREIGN KEY([BankAccountId])
REFERENCES [dbo].[DbBankAccount] ([BankAccountId])
GO
ALTER TABLE [dbo].[DbDeposit] CHECK CONSTRAINT [FK_DbDeposit_DbBankAccount]
GO
ALTER TABLE [dbo].[DbDeposit]  WITH CHECK ADD  CONSTRAINT [FK_DbDeposit_DbBankTransaction] FOREIGN KEY([BankTransactionId])
REFERENCES [dbo].[DbBankTransaction] ([BankTransactionId])
GO
ALTER TABLE [dbo].[DbDeposit] CHECK CONSTRAINT [FK_DbDeposit_DbBankTransaction]
GO
ALTER TABLE [dbo].[DbExpense]  WITH CHECK ADD  CONSTRAINT [FK_DbExpense_DbBankAccount] FOREIGN KEY([BankAccountId])
REFERENCES [dbo].[DbBankAccount] ([BankAccountId])
GO
ALTER TABLE [dbo].[DbExpense] CHECK CONSTRAINT [FK_DbExpense_DbBankAccount]
GO
ALTER TABLE [dbo].[DbExpense]  WITH CHECK ADD  CONSTRAINT [FK_DbExpense_DbBankTransaction] FOREIGN KEY([BankTransactionId])
REFERENCES [dbo].[DbBankTransaction] ([BankTransactionId])
GO
ALTER TABLE [dbo].[DbExpense] CHECK CONSTRAINT [FK_DbExpense_DbBankTransaction]
GO
ALTER TABLE [dbo].[DbItem]  WITH CHECK ADD  CONSTRAINT [FK_DbItem_DbCategory] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[DbCategory] ([CategoryId])
GO
ALTER TABLE [dbo].[DbItem] CHECK CONSTRAINT [FK_DbItem_DbCategory]
GO
ALTER TABLE [dbo].[DbItem]  WITH CHECK ADD  CONSTRAINT [FK_DbItem_DbUnit] FOREIGN KEY([UnitId])
REFERENCES [dbo].[DbUnit] ([UnitId])
GO
ALTER TABLE [dbo].[DbItem] CHECK CONSTRAINT [FK_DbItem_DbUnit]
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceHeader]  WITH CHECK ADD  CONSTRAINT [FK_DbPurchaseInvoiceHeader_DbSupplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[DbSupplier] ([SupplierId])
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceHeader] CHECK CONSTRAINT [FK_DbPurchaseInvoiceHeader_DbSupplier]
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceHsnSacDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbPurchaseInvoiceHsnSacDetails_DbPurchaseInvoiceHeader] FOREIGN KEY([InvoiceNo])
REFERENCES [dbo].[DbPurchaseInvoiceHeader] ([InvoiceNo])
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceHsnSacDetails] CHECK CONSTRAINT [FK_DbPurchaseInvoiceHsnSacDetails_DbPurchaseInvoiceHeader]
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbPurchaseInvoiceItemDetails_DbItem] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[DbItem] ([ItemCode])
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceItemDetails] CHECK CONSTRAINT [FK_DbPurchaseInvoiceItemDetails_DbItem]
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbPurchaseInvoiceItemDetails_DbPurchaseInvoiceItemDetails] FOREIGN KEY([Id], [InvoiceNo])
REFERENCES [dbo].[DbPurchaseInvoiceItemDetails] ([Id], [InvoiceNo])
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceItemDetails] CHECK CONSTRAINT [FK_DbPurchaseInvoiceItemDetails_DbPurchaseInvoiceItemDetails]
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbPurchaseInvoiceItemDetails_DbUnit] FOREIGN KEY([UnitId])
REFERENCES [dbo].[DbUnit] ([UnitId])
GO
ALTER TABLE [dbo].[DbPurchaseInvoiceItemDetails] CHECK CONSTRAINT [FK_DbPurchaseInvoiceItemDetails_DbUnit]
GO
ALTER TABLE [dbo].[DbQuotationHeader]  WITH CHECK ADD  CONSTRAINT [FK_DbQuotationHeader_DbCustomer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[DbCustomer] ([CustomerId])
GO
ALTER TABLE [dbo].[DbQuotationHeader] CHECK CONSTRAINT [FK_DbQuotationHeader_DbCustomer]
GO
ALTER TABLE [dbo].[DbQuotationHsnSacDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbQuotationHsnSacDetails_DbQuotationHeader] FOREIGN KEY([QuotationNo])
REFERENCES [dbo].[DbQuotationHeader] ([QuotationNo])
GO
ALTER TABLE [dbo].[DbQuotationHsnSacDetails] CHECK CONSTRAINT [FK_DbQuotationHsnSacDetails_DbQuotationHeader]
GO
ALTER TABLE [dbo].[DbQuotationItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbQuotationItemDetails_DbItem] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[DbItem] ([ItemCode])
GO
ALTER TABLE [dbo].[DbQuotationItemDetails] CHECK CONSTRAINT [FK_DbQuotationItemDetails_DbItem]
GO
ALTER TABLE [dbo].[DbQuotationItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbQuotationItemDetails_DbQuotationHeader] FOREIGN KEY([QuotationNo])
REFERENCES [dbo].[DbQuotationHeader] ([QuotationNo])
GO
ALTER TABLE [dbo].[DbQuotationItemDetails] CHECK CONSTRAINT [FK_DbQuotationItemDetails_DbQuotationHeader]
GO
ALTER TABLE [dbo].[DbQuotationItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbQuotationItemDetails_DbUnit] FOREIGN KEY([UnitId])
REFERENCES [dbo].[DbUnit] ([UnitId])
GO
ALTER TABLE [dbo].[DbQuotationItemDetails] CHECK CONSTRAINT [FK_DbQuotationItemDetails_DbUnit]
GO
ALTER TABLE [dbo].[DbSalesInvoiceHsnSacDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbSalesInvoiceHsnSacDetails_DbSalesInvoiceHeader] FOREIGN KEY([InvoiceNo])
REFERENCES [dbo].[DbSalesInvoiceHeader] ([InvoiceNo])
GO
ALTER TABLE [dbo].[DbSalesInvoiceHsnSacDetails] CHECK CONSTRAINT [FK_DbSalesInvoiceHsnSacDetails_DbSalesInvoiceHeader]
GO
ALTER TABLE [dbo].[DbSalesInvoiceItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbSalesInvoiceItemDetails_DbItem] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[DbItem] ([ItemCode])
GO
ALTER TABLE [dbo].[DbSalesInvoiceItemDetails] CHECK CONSTRAINT [FK_DbSalesInvoiceItemDetails_DbItem]
GO
ALTER TABLE [dbo].[DbSalesInvoiceItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbSalesInvoiceItemDetails_DbSalesInvoiceHeader] FOREIGN KEY([InvoiceNo])
REFERENCES [dbo].[DbSalesInvoiceHeader] ([InvoiceNo])
GO
ALTER TABLE [dbo].[DbSalesInvoiceItemDetails] CHECK CONSTRAINT [FK_DbSalesInvoiceItemDetails_DbSalesInvoiceHeader]
GO
ALTER TABLE [dbo].[DbSalesInvoiceItemDetails]  WITH CHECK ADD  CONSTRAINT [FK_DbSalesInvoiceItemDetails_DbUnit] FOREIGN KEY([UnitId])
REFERENCES [dbo].[DbUnit] ([UnitId])
GO
ALTER TABLE [dbo].[DbSalesInvoiceItemDetails] CHECK CONSTRAINT [FK_DbSalesInvoiceItemDetails_DbUnit]
GO
ALTER TABLE [dbo].[DbSupplierLedger]  WITH CHECK ADD  CONSTRAINT [FK_DbSupplierLedger_DbBankAccount] FOREIGN KEY([BankAccountId])
REFERENCES [dbo].[DbBankAccount] ([BankAccountId])
GO
ALTER TABLE [dbo].[DbSupplierLedger] CHECK CONSTRAINT [FK_DbSupplierLedger_DbBankAccount]
GO
ALTER TABLE [dbo].[DbSupplierLedger]  WITH CHECK ADD  CONSTRAINT [FK_DbSupplierLedger_DbBankTransaction] FOREIGN KEY([BankTransactionId])
REFERENCES [dbo].[DbBankTransaction] ([BankTransactionId])
GO
ALTER TABLE [dbo].[DbSupplierLedger] CHECK CONSTRAINT [FK_DbSupplierLedger_DbBankTransaction]
GO
ALTER TABLE [dbo].[DbSupplierLedger]  WITH CHECK ADD  CONSTRAINT [FK_DbSupplierLedger_DbPurchaseInvoiceHeader] FOREIGN KEY([InvoiceNo])
REFERENCES [dbo].[DbPurchaseInvoiceHeader] ([InvoiceNo])
GO
ALTER TABLE [dbo].[DbSupplierLedger] CHECK CONSTRAINT [FK_DbSupplierLedger_DbPurchaseInvoiceHeader]
GO
ALTER TABLE [dbo].[DbSupplierLedger]  WITH CHECK ADD  CONSTRAINT [FK_DbSupplierLedger_DbSupplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[DbSupplier] ([SupplierId])
GO
ALTER TABLE [dbo].[DbSupplierLedger] CHECK CONSTRAINT [FK_DbSupplierLedger_DbSupplier]
GO
