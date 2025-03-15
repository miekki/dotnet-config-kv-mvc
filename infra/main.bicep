// targetScope = 'subscription'

// create a resource group called "config-sample-mvc-rg" in uksouth location
// resource configSampleRg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
//   name: 'config-sample-mvc-rg'
//   location: 'uksouth'
// }

// create a key vault called "config-sample-mvc-kv" in the "config-sample-mvc-rg" resource group
resource configSampleKv 'Microsoft.KeyVault/vaults@2024-04-01-preview' = {
  name: 'config-sample-mvc-kv'
  location: 'uksouth'
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableSoftDelete: false
    // Remove accessPolicies as we are using RBAC
    accessPolicies: []
    enableRbacAuthorization: true
  }
}

// create a web app plan called "config-sample-mvc-plan" in the "config-sample-mvc-rg" resource group
resource configSamplePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  //scope: configSampleRg
  name: 'config-sample-mvc-plan'
  location: 'uksouth'
  properties: {
    reserved: true
  }
  sku: {
    tier: 'Basic'
    name: 'B1'
  }
}

// create a web app called "config-sample-mvc-app" in the "config-sample-mvc-rg" resource group
resource configSampleApp 'Microsoft.Web/sites@2024-04-01' = {
  name: 'config-sample-mvc-app'
  location: 'uksouth'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: configSamplePlan.id
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|9.0'
      appSettings: [
        {
          name: 'KeyVaultConfig__KeyVaultUrl'
          value: configSampleKv.properties.vaultUri
        }
      ]
    }
  }
}

// Assign Key Vault Secrets User role to the web app's managed identity
resource keyVaultRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(configSampleKv.id, 'KeyVaultReader', configSampleApp.id)
  properties: {
    principalId: configSampleApp.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '4633458b-17de-408a-b874-0445c86b69e6'
    ) // Key Vault Secrets User
  }
}
