<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Malden.Portal.GUI.Azure" generation="1" functional="0" release="0" Id="ffdbe7df-c245-487f-8476-aaa9baa64cb1" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="Malden.Portal.GUI.AzureGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Malden.Portal.GUI.Azure.Webrole:Endpoint1" protocol="https">
          <inToChannel>
            <lBChannelMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/LB:Malden.Portal.GUI.Azure.Webrole:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/LB:Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="Certificate|Malden.Portal.GUI.Azure.Webrole:MaldenCertificate" defaultValue="">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapCertificate|Malden.Portal.GUI.Azure.Webrole:MaldenCertificate" />
          </maps>
        </aCS>
        <aCS name="Certificate|Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" defaultValue="">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapCertificate|Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </maps>
        </aCS>
        <aCS name="Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </maps>
        </aCS>
        <aCS name="Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </maps>
        </aCS>
        <aCS name="Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </maps>
        </aCS>
        <aCS name="Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </maps>
        </aCS>
        <aCS name="Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </maps>
        </aCS>
        <aCS name="Malden.Portal.GUI.Azure.WebroleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/MapMalden.Portal.GUI.Azure.WebroleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Malden.Portal.GUI.Azure.Webrole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput">
          <toPorts>
            <inPortMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </toPorts>
        </lBChannel>
        <sFSwitchChannel name="SW:Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp">
          <toPorts>
            <inPortMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
          </toPorts>
        </sFSwitchChannel>
      </channels>
      <maps>
        <map name="MapCertificate|Malden.Portal.GUI.Azure.Webrole:MaldenCertificate" kind="Identity">
          <certificate>
            <certificateMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/MaldenCertificate" />
          </certificate>
        </map>
        <map name="MapCertificate|Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" kind="Identity">
          <certificate>
            <certificateMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
          </certificate>
        </map>
        <map name="MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" kind="Identity">
          <setting>
            <aCSMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" />
          </setting>
        </map>
        <map name="MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" kind="Identity">
          <setting>
            <aCSMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" />
          </setting>
        </map>
        <map name="MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" kind="Identity">
          <setting>
            <aCSMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" />
          </setting>
        </map>
        <map name="MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" />
          </setting>
        </map>
        <map name="MapMalden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" kind="Identity">
          <setting>
            <aCSMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" />
          </setting>
        </map>
        <map name="MapMalden.Portal.GUI.Azure.WebroleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.WebroleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Malden.Portal.GUI.Azure.Webrole" generation="1" functional="0" release="0" software="E:\SourceCode\Malden.Portal\Malden.Portal.GUI.Azure\csx\Debug\roles\Malden.Portal.GUI.Azure.Webrole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="https" portRanges="443">
                <certificate>
                  <certificateMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/MaldenCertificate" />
                </certificate>
              </inPort>
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" protocol="tcp" />
              <inPort name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp" portRanges="3389" />
              <outPort name="Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" protocol="tcp">
                <outToChannel>
                  <sFSwitchChannelMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/SW:Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp" />
                </outToChannel>
              </outPort>
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Malden.Portal.GUI.Azure.Webrole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Malden.Portal.GUI.Azure.Webrole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteAccess.Rdp&quot; /&gt;&lt;e name=&quot;Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
            <storedcertificates>
              <storedCertificate name="Stored0MaldenCertificate" certificateStore="CA" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/MaldenCertificate" />
                </certificate>
              </storedCertificate>
              <storedCertificate name="Stored1Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" certificateStore="My" certificateLocation="System">
                <certificate>
                  <certificateMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole/Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
                </certificate>
              </storedCertificate>
            </storedcertificates>
            <certificates>
              <certificate name="MaldenCertificate" />
              <certificate name="Microsoft.WindowsAzure.Plugins.RemoteAccess.PasswordEncryption" />
            </certificates>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.WebroleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.WebroleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.WebroleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="Malden.Portal.GUI.Azure.WebroleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="Malden.Portal.GUI.Azure.WebroleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="Malden.Portal.GUI.Azure.WebroleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="b556ab18-fdaa-410c-bb6b-8bcee15b9873" ref="Microsoft.RedDog.Contract\ServiceContract\Malden.Portal.GUI.AzureContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="d3555eff-df8f-4cb4-884e-73c23bb4e942" ref="Microsoft.RedDog.Contract\Interface\Malden.Portal.GUI.Azure.Webrole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="b74c78ad-5a7c-473d-9f43-ac3f7124698e" ref="Microsoft.RedDog.Contract\Interface\Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Malden.Portal.GUI.Azure/Malden.Portal.GUI.AzureGroup/Malden.Portal.GUI.Azure.Webrole:Microsoft.WindowsAzure.Plugins.RemoteForwarder.RdpInput" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>