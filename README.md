<h1>gbXMLReadWrite</h1>

An open-source C# project that creates valid gbXML per XSD version 5.11
<br><Br>
Created by Chien Si Harriman for Carmel Software Corporation on behalf of gbXML.org. Funding provided by Pacific Northwest National Labs. Thanks to Krishnan Gowri and Amir Roth for their support.
<br><Br>
This is a clean set of classes that can be used to create a gbXML file to describe the geometry and thermal properties of a building for energy simulation. There are no classes present for air loops, and HVAC definitions at this time, although this work is already underway and will be included in the future.
<br><Br>
To see these classes and productivity classes in action, you may want to refer to an implementation for Grasshopper, called Grizzly Bear (part of Mostapha Roudsari's Honeybee project...search for Honeybee in Github and you will find Grizzly Bear as a dependent module in Honeybee). Grizzly Bear is a working example of gbXML serialization transforming a set of base classes in another format to gbXML. You can witness how information needs to be gathered, and the flow of XML creation. This is the best gbxml.org-approved example that we currently have in an open-source format for gbXML so that others can learn how to produce the base set of elements to describe building geometry and thermal properties for energy simulation.
