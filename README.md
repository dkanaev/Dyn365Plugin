# Dyn365Plugin
The sample of creation plugin for dynamics365.
# Description
Plugin designed to work with TimeEntry.
If the date start and the date end is different then it read existing data. Then the data is analyzed to avoid creating duplicates. The plugin should be executed in synchronous (in the transaction) mode to roll back the creation of duplicated data. If needed data will be created for every date from the range of "target" TimeEntry. Due to the fact, that the plugin executes before creating the "target" entity, we have to create all records except one. The target entity is the last missing entry and will be created in the next "Creation" step.
# Installation
To work correctly when installing the plugin by PluginRegistration tool, select the following parameters:

| Parameter                        | Value          |
| ---------------------------------|:--------------:|
| Message                          | Create         |
| Primary Entity                   | msdyn_timeentry|
| Event Pipeline Stage of Execution| PreOperation   |
| Execution Mode                   | Synchronous    |
# Notes
- This plugin will not work if it is run from another plugin.
- This plugin work in UTC timezone. For correct UI testing, I changed the timezone in my profile  to UTC. I also took this into account in unit tests.
- The main logic code can be split(improved)  but it located in one place for clarity.
# Testing
- The plugin installed [here](https://org4fc15751.crm4.dynamics.com/main.aspx?appid=eb9f0695-9b82-eb11-b1ab-000d3aba600e&pagetype=entitylist&etn=msdyn_timeentry)
k4ndmitr@k4ndmitr.onmicrosoft.com / ip777_880
- Additionally, 3 unit tests were added to test the only main cases.

