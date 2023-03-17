# S1XViewer
A viewer for Windows .NET 6.0 for viewing S1xx files, currently only supporting S111 (DCF2, DCF3, DCF8), S122, S123, S127 and S128 GML formats. Support for S102, S104 and S111 (DCF1, DCF4) is under development. This S1XViewer supersedes the [S1xxViewer](https://github.com/flappah/s1xxviewer). 

First implementation. The viewer has different parsers and renderers for IHO's S111 (DCF2, DCF3, DCF8), S122, S123, S127 and S128 GML files. For presentation it uses the ESRI ArcGIS Runtime and currently is using the developer license only.

The application been tested with a number of S111 datacodingformat = 2, S111 datacodingformat = 3, S111 datacodingformat = 8, S122, S123 and S128 GML files.

The software expects an active Internet connection for the ESRI ArcGIS runtime to be able to retrieve the basemap.

Next are a number of screenshots displaying the different IHO S1x standard that are currently supported. The first screenshot shows a sample S122 file.

![S122_sample](https://user-images.githubusercontent.com/14106566/225307603-a6819ad0-3d78-4955-821b-879a87643d67.png)

The next shows a sample S123 file.

![s123_sample](https://user-images.githubusercontent.com/14106566/225308336-b789bbe9-adba-4fb6-99cd-a5e181df5d56.png)

The next screenshow is a sample S128 file.

![S128_sample](https://user-images.githubusercontent.com/14106566/225308463-1ac81923-42c7-4408-88f6-e2b4c81c7001.png)

Next are three S111 screenshots. The first is a datacoding format = 2 file, the second is a datacoding format = 3 file and the third is a datacoding format = 8 file.

![s111_dcf2_sample](https://user-images.githubusercontent.com/14106566/225308576-9d00956c-4ee0-4301-8f8c-864aa3202210.png)

![s111_dcf3_sample](https://user-images.githubusercontent.com/14106566/225870785-c367a86d-fcec-4d7c-a9be-61b7fd270ed3.png)

![S111_dcf8_sample](https://user-images.githubusercontent.com/14106566/225308598-3d99d3ab-c641-4d68-906b-32a9fefd713a.png)
