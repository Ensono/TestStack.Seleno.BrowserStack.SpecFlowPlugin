# TestStack.Seleno.BrowserStack.SpecFlowPlugin

- Custom plugin to generate [NUnit](http://www.nunit.org/) test to support multiple browser configurations on [BrowserStack](https://www.browserstack.com) using [ Seleno](http://seleno.teststack.net/)

- Test class generator to drive automated web ui tests on browser stack with [ Seleno](http://seleno.teststack.net/) and [SpecFlow](http://www.specflow.org/).

- Configures [SpecFlow](http://www.specflow.org/) automatically using [SpecFlow](http://www.specflow.org/)'s [BoDi](https://github.com/gasparnagy/BoDi) Container to be able to get browser.

- Creating automated web tests to test an application in addition to testing the application with unit tests is a good practice. [SpecFlow](http://www.specflow.org/) supports behavior driven development and acceptance tests driven development.

- This project was created to be able to use [ Seleno](http://seleno.teststack.net/) with [SpecFlow](http://www.specflow.org/) as easily as possible, and at the same time to be able to use it in a Continous Integration Environment.

### Release notes:

1.1) Multiple browser support with just tag placed on scenario like `@browser:firefox,43.0,OS_X,Lion`

1.2) Error notification done with after scenario hook and browser stack API to pass error message.

1.3) configure plugin in App.config and set app settings for browser stack basic settings:
```
<configuration>
    <specFlow>
        <plugins>
      <add name="TestStack.Seleno.BrowserStack" path="..\packages\TestStack.Seleno.BrowserStack.SpecFlowPlugin.1.0.3.29167\lib\net451" type="GeneratorAndRuntime" />
    </plugins>
    <stepAssemblies>
      <stepAssembly assembly="TestStack.Seleno.BrowserStack.Core" />
    </stepAssemblies>
    </specFlow>
	<appSettings>
    <add key="browserStackRemoteDriverUrl" value="http://hub.browserstack.com/wd/hub/" />
    <add key="buildNumber" value="" />
    <add key="browserStackApiUrl" value="https://www.browserstack.com/automate/" />
    <!--
     <add key="browserStack.user" value="<yourUserNameHere>" />
     <add key="browserstack.key" value="<yourAccessKeyHere>" />
    -->
  </appSettings>
</configuration>
```
1.4) support resolution for desktop like `@browser:chrome,50.0,Windows,10,1024x768`

1.5) Maximize browser window to use full available resolution specified into browser stack capabilities (default 1024x768)

### Features:


- Sets SpecFlow tests up to run and teardown a [BrowserStack](https://www.browserstack.com)
    - The Browser is a tool which doesn't belong to your application, why write code to instantiate it?

- Annotate scenarios with scenario supporting Browsers
    -  Does every scenario describing the acceptance tests for every browser? Mark the scenario with the supported Browser.

- Adds the browser name as a TestCategory
    - Just run the tests with the categories for the browser you actually have on the environment. Example: Don't run Android browser test as I don't have an android device attached to my machine.


Get it from Nuget.org:

https://www.nuget.org/packages/TestStack.Seleno.BrowserStack.SpecFlowPlugin/
