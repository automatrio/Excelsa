![alt text](https://res.cloudinary.com/automatrio/image/upload/v1628900017/ExcelsaBackground4_jqudtq.png)

# Excelsa
A tiny framework wrapped around Selenium with tools and a project structure to get you started.

### What is it, really?

It's a set of tools designed to facilitate writing Selenium tests, namely:
 - [X] extension methods to Selenium objects that'll help you get, interact with, and filter out elements from the DOM tree;
 - [X] a template architecture designed to avoid code redundancy when writing mostly repetitive tests;
 - [X] an HTML log generator that outputs a pretty and interactable log file after tests have ended;
 - [X] a boilerplate code generator that creates the basic files you need to start writing tests;
 - [X] a colorful console-application to help you generate said code and configure your own project.

It is named after the Brazil nut *(Bertholletia **excelsa**)*, known to be rich in selenium.
 
### When is it useful?

Excelsa was initially created from the necessity of writing an extensive number of **tests** that contemplated different, and yet similar looking **pages**. 
These pages varied only in some of the **components** they featured, in the position they'd show up in the sets of data each presented (i.e., in their **specification**).
Thus, Excelsa's architecture imitates this simple, almost universal model.

### How can I install it?

To use it, you should clone this repository and install its dependencies: NUnit and Selenium NuGet packages.

### How can I use it?

Run the Console Application from your code editor, or via a simple console "dotnet run" if you prefer, and it'll launch the application.
And that's it. From there, you'll be able to easily configure your project and generate all the files you'll need.

### How will it evolve?

Excelsa is a bare-bones framework right now, and it is pretty skeletal in its own nature, really, but that doesn't mean the work is done.
Here are some of the features it could most certainly do with and I intend to implement:
- [ ] switching to XUnit, instead of NUnit, thus enabling .NET Core and running on different OS's;
- [ ] import and export of Excel, or libre-equivalent, files;
- [ ] more extension methods to enable faster creation of tests;

### Contributors

Excelsa was written by me, but the inspiration came from Ricardo Oliveira's original architecture at Target Software, Sorocaba, both of which
I owe much gratitude to for their support and encouragement.
