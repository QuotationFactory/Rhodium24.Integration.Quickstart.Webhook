# Rhodium24 Integration Quickstart For Webhooks

This quickstart is build for using the webhook integration feature of Rhodium24.

## How to run

**Important:** You will need to retrieve some information from Rhodium24 support in order to be able to use the Rhodium24 API. These keys need to be configured in the appsettings.Development.json file.

You will have to configure these settings:
- ApiUrl
- SubscriptionKey
- ClientId
- ClientSecret

If those keys are configured you can test the webhooks with the following steps:

1. Run the 'Rhodium24.Integration.Api' with default configuration (http port 5000, https port 5001)
2. Execute 'Start ngrok.bat' in root of this project (this wil create a public proxy URL to the local API on port 5000)
3. Copy the **https** forwarding URL from the console (Example: https://8428-2a02-a454-55fa-1-7547-9407-5f74-a456.ngrok.io)
4. Navigate to [Rhodium24 webhooks config page](https://www.rhodium24.io/app/settings/integrations/webhooks) (Loading can take a few minutes the first time)
5. Add an endpoint or update an existing endpoint with the url you copied in step 3 and add '/webhook' to it.
   - Example: https://8428-2a02-a454-55fa-1-7547-9407-5f74-a456.ngrok.io/webhook
6. Change the state of a project and see the API in action