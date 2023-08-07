# S1XViewer
A viewer for Windows .NET 6.0 for viewing S1xx files, currently supporting S102 (DCF2), S104 (DCF1, DCF2, DCF3, DCF8), S111 (DCF1, DCF2, DCF3, DCF4, DCF8), S122, S123 and S128 GML formats. This S1XViewer supersedes the [S1xxViewer](https://github.com/flappah/s1xxviewer). 

The viewer has different parsers and renderers for IHO's S102 (DCF2), S104 (DCF1, DCF2, DCF3, DCF8), S111 (DCF1, DCF2, DCF3, DCF4, DCF8), S122, S123 and S128 GML files. For presentation it uses the ESRI ArcGIS Runtime and currently is using the developer license only.

The application been tested with a number of S102 (datacodingformat = 2), S104 (datacodingformat = 1, datacodingformat = 2, datacodingformat = 3, datacodingformat = 8), S111 (datacodingformat = 1, datacodingformat = 2, S111 datacodingformat = 3, datacodingformat = 4, datacodingformat = 8), S122, S123 and S128 GML files. 

The software expects an active Internet connection for the ESRI ArcGIS runtime to be able to retrieve the basemap.

>**There is an issue with the installer in regards to the existence of older version of the viewer. To get the best results its better to manually uninstall the previous version before installing this newer version!!**

Next are a number of screenshots displaying the different IHO S1x standard that are currently supported. The first screenshot shows a sample S122 file.

![S122_sample](https://user-images.githubusercontent.com/14106566/225307603-a6819ad0-3d78-4955-821b-879a87643d67.png)

The next shows a sample S123 file.

![s123_sample](https://user-images.githubusercontent.com/14106566/225308336-b789bbe9-adba-4fb6-99cd-a5e181df5d56.png)

The next screenshow is a sample S128 file.

![S128_sample](https://user-images.githubusercontent.com/14106566/225308463-1ac81923-42c7-4408-88f6-e2b4c81c7001.png)

Next are five S111 screenshots for every datacodingformat one screenshot. The first is a datacoding format = 1 file, the second is a datacoding format = 2 file, the third is a datacoding format = 3, the fourth is a datacoding format = 4 and the fifth is a datacoding format = 8 file.

![S111_DCF1_screenshot](https://user-images.githubusercontent.com/14106566/233617206-25fcb50b-c914-4fae-8ef9-9d682e9bccd4.png)

![s111_dcf2_sample](https://user-images.githubusercontent.com/14106566/225308576-9d00956c-4ee0-4301-8f8c-864aa3202210.png)

![s111_dcf3_sample](https://user-images.githubusercontent.com/14106566/225870785-c367a86d-fcec-4d7c-a9be-61b7fd270ed3.png)

![S111_DCF4_screenshot](https://user-images.githubusercontent.com/14106566/233617251-aed58f0d-0f47-4014-bd1b-f8d9ed264702.png)

![S111_dcf8_sample](https://user-images.githubusercontent.com/14106566/225308598-3d99d3ab-c641-4d68-906b-32a9fefd713a.png)

There is support for S104 dcf2 and dcf8. It roughly follows the portrayal guidelines specified in the S104 v1.2 standard. It shows 
a popup with the current tidal height information at the specified time. It renders a link that displays the full 24Hr tidal curve 
for the selected tidal station. Here's a screenshot of a loaded S104 DCF2 file.

![S104_DCF2_screenshot_plus_popup](https://github.com/flappah/S1XViewer/assets/14106566/3647ffc2-adb3-459e-aaad-3a68a09b624c)

Next are two screenshots of a loaded S104 DCF8 file.

![S104_DCF8_screenshot_plus_popup](https://github.com/flappah/S1XViewer/assets/14106566/8f5252d9-86e0-4831-9f83-4e6cdf6b895f)

When the link is clicked on the full tidal curve is displayed.

![S104_DCF8_screenshot_plus_tidal_curve](https://github.com/flappah/S1XViewer/assets/14106566/fe8d8caa-5ea1-4b46-a329-98a0c4b11993)

Finally there's an S102 screenshot. It's a datacoding format = 2 sample.

![s102_dcf2_sample](https://user-images.githubusercontent.com/14106566/226903436-92f05742-029d-455f-b6b8-089a72434b53.png)

The viewer currently has limited support for multiple layers. It does however support simultaneously viewing multiple S102 files on top of an S57 ENC. The following screenshot show just that.

![NLHO_S102_over_ENC](https://user-images.githubusercontent.com/14106566/232773044-0fb0e829-9919-4d0b-86c0-290aeb9d7d88.png)

