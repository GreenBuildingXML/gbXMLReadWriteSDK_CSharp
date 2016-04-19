using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace gbXMLSerializer
{
    class EnergyPlusClass
    {
        public class ZoneGroup
        {
            public string name;
            ZoneList zoneList;
            public int zoneListMultiplier;
            public string zoneListName;
        }

        public class ZoneList
        {
            public List<string> zoneListNames;
            public string name;
        }

        static public List<List<double>> GetCoordinDoubles(List<EPObj.MemorySafe_CartCoord> coords)
        {
            List<List<double>> returncoords = new List<List<double>>();
            foreach (EPObj.MemorySafe_CartCoord coord in coords)
            {
                List<double> coordinstance = new List<double>();
                coordinstance.Add(coord.X);
                coordinstance.Add(coord.Y);
                coordinstance.Add(coord.Z);
                returncoords.Add(coordinstance);
            }

            return returncoords;

        }

        static public double FindTilt(EPObj.MemorySafe_CartVect normalVector)
        {
            double calculatedTilt = -999;
            //may need to also take into account other factors that, at this stage, seem to not be important
            //building Direction of Relative North
            //zone Direction of Relative North
            //GlobalGeometryRules coordinate system
            //I may need to know this in the future then rotate the axis vectors I am making below

            //x-axis [1 0 0] points east, y-axis [0 1 0] points north, z-axis[0 0 1] points up to the sky
            //alignment with y axis means north pointing, alignment with z-axis means it is pointing up to the sky (like a flat roof)
            double nX = 0;
            double nY = 1;
            double nZ = 0;
            EPObj.MemorySafe_CartVect northVector = new EPObj.MemorySafe_CartVect(nX, nY, nZ);

            double uX = 0;
            double uY = 0;
            double uZ = 1;
            EPObj.MemorySafe_CartVect upVector = new EPObj.MemorySafe_CartVect(uX, uY, uZ);

            //rotate the axis vectors for the future

            //ensure the vector passed into the function is a unit vector
            normalVector = EPObj.UnitVector(normalVector);
            //get tilt:  cross product of normal vector and upVector
            //since parallel and anti parallel vectors will return the same cross product [0,0,0] I need to filter out the antiparalll case
            if (normalVector.X == upVector.X * -1 && normalVector.Y == upVector.Y * -1 && normalVector.Z == upVector.Z * -1)
            {
                calculatedTilt = 180;
                return calculatedTilt;
            }
            else
            {
                EPObj.MemorySafe_CartVect tiltVector = EPObj.CrossProduct(normalVector, upVector);
                double tiltVectorMagnitude = EPObj.VectorMagnitude(tiltVector);
                calculatedTilt = Math.Round(Math.Asin(tiltVectorMagnitude) * 180 / Math.PI, 2);
                return calculatedTilt;
            }
        }

        static public double FindAzimuth(EPObj.CartVect normalVector)
        {
            double calculatedAzimuth = -999;
            //may need to also take into account other factors that, at this stage, seem to not be important
            //building Direction of Relative North
            //zone Direction of Relative North
            //GlobalGeometryRules coordinate system
            //I may need to know this in the future then rotate the axis vectors I am making below

            //x-axis [1 0 0] points east, y-axis [0 1 0] points north, z-axis[0 0 1] points up to the sky
            //alignment with y axis means north pointing, alignment with z-axis means it is pointing up to the sky (like a flat roof)

            EPObj.CartVect northVector = new EPObj.CartVect();
            northVector.X = 0;
            northVector.Y = 1;
            northVector.Z = 0;


            EPObj.CartVect southVector = new EPObj.CartVect();
            southVector.X = 0;
            southVector.Y = -1;
            southVector.Z = 0;

            EPObj.CartVect eastVector = new EPObj.CartVect();
            eastVector.X = 1;
            eastVector.Y = 0;
            eastVector.Z = 0;

            EPObj.CartVect westVector = new EPObj.CartVect();
            westVector.X = -1;
            westVector.Y = 0;
            westVector.Z = 0;

            EPObj.CartVect upVector = new EPObj.CartVect();
            upVector.X = 0;
            upVector.Y = 0;
            upVector.Z = 1;


            //rotate the axis vectors for the future

            //ensure the vector passed into the function is a unit vector
            normalVector = EPObj.UnitVector(normalVector);
            //get X-Y projection of the normal vector
            normalVector.Z = 0;
            //get azimuth:  cross product of normal vector x-y projection and northVector
            //1st quadrant
            if ((normalVector.X == 0 && normalVector.Y == 1) || (normalVector.X == 1 && normalVector.Y == 0) || (normalVector.X > 0 && normalVector.Y > 0))
            {
                //get azimuth:  cross product of normal vector x-y projection and northVector
                EPObj.CartVect azVector = EPObj.CrossProduct(normalVector, northVector);
                double azVectorMagnitude = EPObj.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2);
                return calculatedAzimuth;
            }
            //second quadrant
            else if (normalVector.X < 0 && normalVector.Y > 0)
            {
                EPObj.CartVect azVector = EPObj.CrossProduct(normalVector, westVector);
                double azVectorMagnitude = EPObj.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 270;
                return calculatedAzimuth;
            }
            //quadrant 3
            else if ((normalVector.X < 0 && normalVector.Y < 0) || (normalVector.X == -1 && normalVector.Y == 0))
            {
                EPObj.CartVect azVector = EPObj.CrossProduct(normalVector, southVector);
                double azVectorMagnitude = EPObj.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 180;
                return calculatedAzimuth;
            }
            //quadrant 4
            else if ((normalVector.X > 0 && normalVector.Y < 0) || (normalVector.X == 0 && normalVector.Y == -1))
            {
                EPObj.CartVect azVector = EPObj.CrossProduct(normalVector, eastVector);
                double azVectorMagnitude = EPObj.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 90;
                return calculatedAzimuth;
            }
            //this will happen to vectors that point straight down or straight up because we are only interested in the X-Y projection and set the Z to zero anyways
            else if (normalVector.X == 0 && normalVector.Y == 0 && normalVector.Z == 0)
            {
                calculatedAzimuth = 0;
                return calculatedAzimuth;
            }

            //get the 

            return calculatedAzimuth;
        }

        public class TagEndings
        {
            public class DetFenEndings
            {
                public static string Name = "!- Name";
                public static string Type = "!- Surface Type";
                public static string ConstName = "!- Construction Name";
                public static string BldSurfName = "!- Building Surface Name";
                public static string OutsideBound = "!- Outside Boundary Condition Object";
                public static string viewFactor = "!- View Factor to Ground";
                public static string shadeControl = "!- Shading Control Name";
                public static string frameAndDivider = "!- Frame and Divider Name";
                public static string multiplier = "!- Multiplier";
                public static string numberVertices = "!- Number of Vertices";
            }

            public class SizingZoneEndings
            {
                public static string outdoorAirMethod = "!- Outdoor Air Method";
                public static string outdoorAirFlowPerson = "!- Outdoor Air Flow per Person";
                public static string outdoorAirFlowPerArea = "!- Outdoor Air Flow per Zone Floor Area";
                public static string outdoorAirFlowPerZone = "!- Outdoor Air Flow per Zone";
            }

            public class LowTempRadiantSurfaceGroupEndings
            {
                public static string surfaceNumName = "!- Surface [0-9]+ Name";
                public static string surfaceFlowFracNum = "!- Flow Fraction for Surface [0-9]+";
            }
        }

        public class EPlusRegexString
        {
            //the start and end of objects...the regexes needed to grab them
            public static string startSurface = "BuildingSurface:Detailed,";
            public static string startFenestration = "FenestrationSurface:Detailed,";
            public static string startWindowFrameDesc = "WindowProperty:FrameAndDivider,";
            public static string startDetailedBuildingShade = "Shading:Building:Detailed,";
            public static string startDesignSpecOA = "DesignSpecification:OutdoorAir,";
            public static string startRadiantSurfaceGroup = "ZoneHVAC:LowTemperatureRadiant:SurfaceGroup";
            public static string commentStart = "(!-).*";
            //super generic
            public static string Name = @"(?'1'.*)(?'Name'!- Name)";
            public static string semicolon = @"(\S*)(;)(.*)";
            public static string constructionName = @"(?'1'.*)(?'construction'!- Construction Name)";
            public static string typicalVertex = @"(?'1'.*)(?'vertices'!- X,Y,Z)";

            //detailed surfaces
            public static string surfaceType = @"(?'1'.*)(?'surfaceType'!- Surface Type)";
            public static string outsideBoundaryName = @"(?'1'.*)(?'outsideBoundaryName'!- Outside Boundary Condition)";

            //detailed fenstration objects
            //start fenestration goes here
            public static string fenestrationName = @"(?'1'.*)(?'fenestrationName'" + TagEndings.DetFenEndings.Name + ")";
            public static string fenestrationType = @"(?'1'.*)(?'fenType'" + TagEndings.DetFenEndings.Type + ")";
            public static string fenConstructionName = @"(?'1'.*)(?'construction'" + TagEndings.DetFenEndings.ConstName + ")";
            public static string parentSurfaceName = @"(?'1'.*)(?'parentSurface'" + TagEndings.DetFenEndings.BldSurfName + ")";
            public static string outsideBoundaryCondition = @"(?'1'.*)(?'outsideBoundaryCondition'" + TagEndings.DetFenEndings.OutsideBound + ")";
            public static string viewFactor2Ground = @"(?'1'.*)(?'viewFactor'" + TagEndings.DetFenEndings.viewFactor + ")";
            public static string shadeControlName = @"(?'1'.*)(?'shadeControl'" + TagEndings.DetFenEndings.shadeControl + ")";
            public static string frameAndDividerName = @"(?'1'.*)(?'frameAndDivider'" + TagEndings.DetFenEndings.frameAndDivider + ")";
            public static string fenestrationMultiplier = @"(?'1'.*)(?'multiplier'" + TagEndings.DetFenEndings.multiplier + ")";
            public static string numberofFenestrationVertices = @"(?'1'.*)(?'vertices'" + TagEndings.DetFenEndings.numberVertices + ")";
            //typical vertex goes here

            //for zones
            public static string startZoneList = "ZoneList,";
            public static string startZoneGroup = "ZoneGroup,";
            public static string zoneListName = @"(?'1'.*)(?'zoneListName'!- Zone List Name)";
            public static string zoneListMultiplier = @"(?'1'.*)(?'multiplier'!- Zone List Multiplier)";
            public static string zoneSizing = "Sizing:Zone,";
            public static string zoneName = @"(?'1'.*)(?'zoneName'!- Zone or ZoneList Name)";

            //for Sizing:Zone, etc
            public static string outdoorAirMethod = @"(?'1'.*)(?'OAMethod'" + TagEndings.SizingZoneEndings.outdoorAirMethod + ")";
            public static string outdoorAirFlowPerson = @"(?'1'.*)(?'OAFlowPerPerson'" + TagEndings.SizingZoneEndings.outdoorAirFlowPerson + ")";
            public static string outdoorAirFlowPerArea = @"(?'1'.*)(?'OAMethod'" + TagEndings.SizingZoneEndings.outdoorAirFlowPerArea + ")";
            public static string outdoorAirFlowPerZone = @"(?'1'.*)(?'OAMethod'" + TagEndings.SizingZoneEndings.outdoorAirFlowPerZone + ")";

            //for ZoneHVAC:LowTemperatureRadiant:SurfaceGroup
            public static string surfaceNumName = @"(?'1'.*)(?'OAMethod'" + TagEndings.LowTempRadiantSurfaceGroupEndings.surfaceNumName + ")";
            public static string surfaceFlowFracNum = @"(?'1'.*)(?'OAMethod'" + TagEndings.LowTempRadiantSurfaceGroupEndings.surfaceFlowFracNum + ")";

        }

        public static List<EPObj.MemorySafe_Spaces> EPlusSpacestoObjectList(string idfname)
        {
            //find multipliers if they exist by looking for Zone Groups
            //I set this up to read the idf file twice, once in each try block
            List<EPObj.MemorySafe_Spaces> memSafeProjectSpaces = new List<EPObj.MemorySafe_Spaces>();
            List<ZoneGroup> zoneGroups = new List<ZoneGroup>();
            List<ZoneList> zoneLists = new List<ZoneList>();
            try
            {
                //may also want to have 
                //the zone list and affiliated spaces should come first

                Regex zoneListYes = new Regex(EPlusRegexString.startZoneList);
                Regex zoneGroupYes = new Regex(EPlusRegexString.startZoneGroup);
                Regex zoneGroupNameRegex = new Regex(EPlusRegexString.Name);
                Regex zoneListNameRegex = new Regex(EPlusRegexString.zoneListName);
                Regex zoneListNameMultiplier = new Regex(EPlusRegexString.zoneListMultiplier);
                Regex semicolon = new Regex(EPlusRegexString.semicolon);

                Encoding encoding;

                using (StreamReader reader = new StreamReader(idfname))
                {
                    string line;
                    encoding = reader.CurrentEncoding;
                    bool zoneListBool = false;
                    bool zoneGroupBool = false;
                    List<string> zoneListStrings = new List<string>();
                    List<string> zoneGroupStrings = new List<string>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        Match zoneListFound = zoneListYes.Match(line);
                        if (zoneListFound.Success)
                        {
                            zoneListBool = true;
                            zoneListStrings.Add(line);
                            continue;
                        }
                        if (zoneListBool)
                        {
                            //parse the line to get the name of the zone
                            Match semiColonMatch = semicolon.Match(line);
                            if (!semiColonMatch.Success)
                            {
                                zoneListStrings.Add(line);

                            }
                            else
                            {
                                zoneListStrings.Add(line);
                                zoneListBool = false;
                                ZoneList zoneList = MakeZoneList(zoneListStrings);
                                zoneLists.Add(zoneList);
                                zoneListStrings.Clear();
                            }
                        }
                        Match zoneGroupFound = zoneGroupYes.Match(line);
                        if (zoneGroupFound.Success)
                        {
                            zoneGroupBool = true;
                            zoneGroupStrings.Add(line);
                            continue;
                        }
                        if (zoneGroupBool)
                        {
                            Match semiColonMatch = semicolon.Match(line);
                            if (!semiColonMatch.Success)
                            {
                                zoneGroupStrings.Add(line);
                            }
                            else
                            {
                                zoneGroupStrings.Add(line);
                                zoneGroupBool = false;
                                ZoneGroup zoneGroup = MakeZoneGroup(zoneGroupStrings);
                                zoneGroups.Add(zoneGroup);
                                zoneGroupStrings.Clear();

                            }
                        }

                        //make a zone List object 

                        //find the zone group that is relate to this list
                    }
                    reader.Close();
                }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //set up the list of spaces that will be collected
            List<EPObj.Spaces> projectSpaces = new List<EPObj.Spaces>();
            //set up the log
            string log = @"C:\Users\Chiensi\Documents\AAATerabuild\EnergyPlus\homework\log.txt";
            StringBuilder logline = new StringBuilder();
            try
            {
                Encoding encoding;
                StringBuilder output = new StringBuilder();
                List<string> stuff = new List<string>();

                //needed regular expressions to build the surface description prior to it being made an object
                string startSurface = "BuildingSurface:Detailed,";
                Regex surfaceYes = new Regex(startSurface);
                string semicolon = @"(\S*)(;)(.*)";
                Regex smicln = new Regex(semicolon);



                using (StreamReader reader = new StreamReader(idfname))
                {
                    string line;
                    encoding = reader.CurrentEncoding;
                    bool detailedsurface = false;
                    //set up the surface
                    EPObj.Surface currentSurface = new EPObj.Surface();
                    currentSurface.SurfaceCoords = new List<EPObj.CartCoord>();
                    currentSurface.surfaceType = EPObj.SurfaceTypes.Blank;
                    currentSurface.outsideBoundary = EPObj.OutsideBoundary.Blank;
                    int surfcount = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        #region
                        MatchCollection surfaceStart = surfaceYes.Matches(line);
                        if (surfaceStart.Count > 0)
                        {
                            detailedsurface = true;
                            currentSurface.Clear();
                            stuff.Add(line);
                            output.AppendLine(line);
                            continue;
                        }
                        //now that a surface element is established in the IDF, we can work through it to create surface objects
                        if (detailedsurface == true)
                        {
                            //GetAllDetailedSurfaces
                            //use streamswriter to make a little text file that will then be turned into an object
                            //write to the small output stream until you encounter a semi-colon
                            stuff.Add(line);
                            output.AppendLine(line);
                            Match smicolMatch = smicln.Match(line);
                            if (smicolMatch.Success)
                            {
                                detailedsurface = false;
                                string pass = output.ToString();
                                //write the output file
                                //send the output file to a function, returning a surface
                                EPObj.Surface surfaceReturned = EPlusSurfacetoObject(stuff);
                                //add a multiplier to the surface if needed
                                foreach (ZoneGroup zoneGroup in zoneGroups)
                                {
                                    string zoneListName = zoneGroup.zoneListName;
                                    foreach (ZoneList zoneList in zoneLists)
                                    {
                                        if (zoneList.name == zoneListName)
                                        {
                                            foreach (string zoneName in zoneList.zoneListNames)
                                            {
                                                if (surfaceReturned.zoneName == zoneName)
                                                {
                                                    //add a multiplier to the surface
                                                    surfaceReturned.multiplier = zoneGroup.zoneListMultiplier;
                                                }
                                            }
                                        }
                                    }
                                }
                                surfcount++;
                                Console.WriteLine(surfcount.ToString());
                                //ModelingUtilities.BuildingObjects.Surface surfaceReturned = EPlusFunctions.EPlusSurfacetoObject("C:\\Temp\\detailedSurface.txt");                                
                                output.Clear();
                                stuff.Clear();
                                if (projectSpaces.Count == 0)
                                {
                                    string tagline = "First project zone detected in Surfaces.";
                                    logline.AppendLine(line);
                                    string zoneName = surfaceReturned.zoneName;
                                    string surfaceName = surfaceReturned.name;
                                    logline.AppendLine(zoneName + ": " + surfaceName);
                                    Console.WriteLine(tagline);
                                    Console.WriteLine(zoneName + ": " + surfaceName);
                                    EPObj.Spaces spaceInstance = new EPObj.Spaces();
                                    spaceInstance.spaceSurfaces = new List<EPObj.Surface>();
                                    spaceInstance.name = surfaceReturned.zoneName;
                                    spaceInstance.spaceSurfaces.Add(surfaceReturned);
                                    projectSpaces.Add(spaceInstance);
                                }
                                else
                                {
                                    //search for the space name in the existing List of Spaces
                                    bool spacefound = false;
                                    for (int i = 0; i < projectSpaces.Count; i++)
                                    {
                                        string projname = projectSpaces[i].name;
                                        if (projname == surfaceReturned.zoneName)
                                        {
                                            string tagline = "Existing project zone detected in Surfaces.";
                                            logline.AppendLine(tagline);
                                            Console.WriteLine(tagline);
                                            string zoneName = surfaceReturned.zoneName;
                                            string surfaceName = surfaceReturned.name;
                                            logline.AppendLine(zoneName + ": " + surfaceName);
                                            Console.WriteLine(zoneName + ": " + surfaceName);
                                            projectSpaces[i].spaceSurfaces.Add(surfaceReturned);
                                            spacefound = true;
                                            break;
                                        }
                                    }
                                    //if spacefound is never set to true
                                    if (!spacefound)
                                    {
                                        string tagline = "New project zone detected in Surfaces.";
                                        logline.AppendLine(tagline);
                                        Console.WriteLine(tagline);
                                        string zoneName = surfaceReturned.zoneName;
                                        string surfaceName = surfaceReturned.name;
                                        logline.AppendLine(zoneName + ": " + surfaceName);
                                        Console.WriteLine(zoneName + ": " + surfaceName);
                                        EPObj.Spaces spaceInstance = new EPObj.Spaces();
                                        spaceInstance.spaceSurfaces = new List<EPObj.Surface>();
                                        spaceInstance.name = surfaceReturned.zoneName;
                                        spaceInstance.spaceSurfaces.Add(surfaceReturned);
                                        projectSpaces.Add(spaceInstance);
                                    }

                                }
                                //semicolon match = true
                            }
                            //detailed surfaces = true
                        }
                        //while line reader is reading
                    }
                    //convert these spaces to memory safe spaces

                    foreach (EPObj.Spaces space in projectSpaces)
                    {
                        string zoneName = space.name;
                        int multiplier = space.multiplier;
                        List<EPObj.MemorySafe_Surface> projectSpaceSurfaces = new List<EPObj.MemorySafe_Surface>();
                        foreach (EPObj.Surface surface in space.spaceSurfaces)
                        {
                            EPObj.MemorySafe_Surface memsurf = EPObj.convert2MemorySafeSurface(surface);
                            projectSpaceSurfaces.Add(memsurf);
                        }

                        EPObj.MemorySafe_Spaces memSpace = new EPObj.MemorySafe_Spaces(zoneName, multiplier, projectSpaceSurfaces);
                        memSafeProjectSpaces.Add(memSpace);

                    }
                    //streamreader
                    reader.Close();
                    using (StreamWriter writer = new StreamWriter(log, false, encoding))
                    {
                        writer.Write(logline.ToString());
                    }
                }

                        #endregion


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return memSafeProjectSpaces;
        }

        private static ZoneGroup MakeZoneGroup(List<string> zoneGroupStrings)
        {
            ZoneGroup newZoneGroup = new ZoneGroup();

            Regex semiColonRegex = new Regex(EPlusRegexString.semicolon);
            Regex nameRegex = new Regex(EPlusRegexString.Name);
            Regex zoneListNameRegex = new Regex(EPlusRegexString.zoneListName);
            Regex zoneListMultiplierRegex = new Regex(EPlusRegexString.zoneListMultiplier);
            try
            {
                foreach (string line in zoneGroupStrings)
                {
                    Match nameMatch = nameRegex.Match(line);
                    if (nameMatch.Success)
                    {
                        string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                        Regex purifyRegex = new Regex(purify);
                        Match pure = purifyRegex.Match(nameMatch.Groups["1"].Value);
                        if (pure.Success)
                        {
                            newZoneGroup.name = pure.Groups["goods"].Value;
                            continue;
                        }
                    }
                    //match the Zone List Name
                    Match zoneListNameMatch = zoneListNameRegex.Match(line);
                    if (zoneListNameMatch.Success)
                    {
                        string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                        Regex purifyRegex = new Regex(purify);
                        Match pure = purifyRegex.Match(zoneListNameMatch.Groups["1"].Value);
                        if (pure.Success)
                        {
                            newZoneGroup.zoneListName = pure.Groups["goods"].Value;
                            continue;
                        }
                    }
                    //match the Zone Multiplier
                    Match zoneListMultiplierMatch = zoneListMultiplierRegex.Match(line);
                    if (zoneListMultiplierMatch.Success)
                    {
                        string purify = @"(?'ws'\s*)(?'goods'.*)(?'semicolon';)";
                        Regex purifyRegex = new Regex(purify);
                        Match pure = purifyRegex.Match(zoneListMultiplierMatch.Groups["1"].Value);
                        if (pure.Success)
                        {
                            newZoneGroup.zoneListMultiplier = Convert.ToInt32(pure.Groups["goods"].Value);
                            continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return newZoneGroup;
        }

        private static ZoneList MakeZoneList(List<string> zoneListStrings)
        {
            ZoneList newZoneList = new ZoneList();
            newZoneList.zoneListNames = new List<string>();
            Regex semiColonRegex = new Regex(EPlusRegexString.semicolon);
            Regex nameRegex = new Regex(EPlusRegexString.Name);
            Regex zoneListStartRegex = new Regex(EPlusRegexString.startZoneList);

            try
            {
                foreach (string line in zoneListStrings)
                {
                    Match semiColonMatch = semiColonRegex.Match(line);
                    if (!semiColonMatch.Success)
                    {
                        Match zoneListStart = zoneListStartRegex.Match(line);
                        if (zoneListStart.Success)
                        {
                            //do not need to do anything.
                            continue;
                        }
                        Match nameMatch = nameRegex.Match(line);
                        if (nameMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(nameMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                newZoneList.name = pure.Groups["goods"].Value;
                                continue;
                            }
                        }
                        //match any generic line that does not have a semicolon in it
                        //dangerous as this will greedily match just about anything
                        string purifyList = @"(?'ws'\s*)(?'goods'.*)(?'comma',)(?'stuff'.*)";
                        Regex purifyRegex2 = new Regex(purifyList);
                        Match pureList = purifyRegex2.Match(line);
                        if (pureList.Success)
                        {
                            string zoneName = pureList.Groups["goods"].Value;
                            newZoneList.zoneListNames.Add(zoneName);
                            continue;
                        }
                    }
                    else
                    {
                        //string purification with a semicolong instead of a comma
                        string purifyList = @"(?'ws'\s*)(?'goods'.*)(?'semicolon';)(?'stuff'.*)";
                        Regex purifyRegex2 = new Regex(purifyList);
                        Match pureList = purifyRegex2.Match(line);
                        if (pureList.Success)
                        {
                            string zoneName = pureList.Groups["goods"].Value;
                            newZoneList.zoneListNames.Add(zoneName);
                            continue;
                        }

                    }
                }
                return newZoneList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return newZoneList;
        }


        static public EPObj.Surface EPlusSurfacetoObject(List<string> detailedSurfaceString)
        {
            //create your log file writer, that will be used in stream writer at the bottom of this page
            
            StringBuilder output = new StringBuilder();
            //need to add a try/catch clause and start to work on try/catches when I get the chance

            //initialize the surface to be returned
            EPObj.Surface currentSurface = new EPObj.Surface();
            currentSurface.SurfaceCoords = new List<EPObj.CartCoord>();
            currentSurface.surfaceType = EPObj.SurfaceTypes.Blank;
            currentSurface.outsideBoundary = EPObj.OutsideBoundary.Blank;

            StringBuilder logline = new StringBuilder();
            //Constructions - Opaque Detailed...

            #region
            //start with the Regexes needed to parse out the opaque constructions
            //Regex for beginning of a detailed surface element,
            string startSurface = "BuildingSurface:Detailed,";
            Regex surfaceYes = new Regex(startSurface);
            //Regex for surfaceName
            //string surfaceName = @"(?'ws1'\s*)(?'name'.*)(?'ws2'\s*)(?'surfaceName'!- Name)";
            string surfaceName = @"(?'1'.*)(?'surfaceName'!- Name)";
            //string surfaceName = @"(?'surfaceName'!- Name)";
            Regex surfaceNameRegex = new Regex(surfaceName);
            //Regex for surface Type
            string surfaceType = @"(?'1'.*)(?'surfaceType'!- Surface Type)";
            Regex surfaceTypeRegex = new Regex(surfaceType);
            //Regext for surfaceConstructionName
            string surfaceConstructionName = @"(?'1'.*)(?'construction'!- Construction Name)";
            Regex surfaceConstructionNameRegex = new Regex(surfaceConstructionName);
            //Regext for surfaceZoneName
            string surfaceZoneName = @"(?'1'.*)(?'zoneName'!- Zone Name)";
            Regex surfaceZoneNameRegex = new Regex(surfaceZoneName);
            //Regex for outsideBoundary
            string outsideBoundaryName = @"(?'1'.*)(?'outsideBoundaryName'!- Outside Boundary Condition)";
            Regex outsideBoundaryRegex = new Regex(outsideBoundaryName);
            //Regex for outsideBoundary Condition
            string outsideBoundaryCondition = @"(?'1'.*)(?'outsideBoundaryCondition'!- Outside Boundary Condition Object)";
            Regex outsideBoundaryConditionRegex = new Regex(outsideBoundaryCondition);
            //Regex for sunExposure
            string sunExposure = @"(?'1'.*)(?'sunExposure'!- Sun Exposure)";
            Regex sunExposureRegex = new Regex(sunExposure);
            //Regex for windExposure
            string windExposure = @"(?'1'.*)(?'windExposure'!- Wind Exposure)";
            Regex windExposureRegex = new Regex(windExposure);
            //Regex for ViewFactor
            string viewFactor = @"(?'1'.*)(?'viewFactor'!- View Factor to Ground)";
            Regex viewFactorRegex = new Regex(viewFactor);
            //Regex for numberofVertices
            string numberofVertices = @"(?'1'.*)(?'vertices'!- Number of Vertices)";
            Regex numberofVerticesRegex = new Regex(numberofVertices);
            //something else for the vertex
            //Regex for generic Vertex
            string typicalVertex = @"(?'1'.*)(?'Xvertex'!- X[0-9]*),\s*(?'Yvertex'Y[0-9]*),\s*(?'ZVertex'Z[0-9]*)\s*";
            Regex typicalVertexRegex = new Regex(typicalVertex);

            //a regex for finding a semicolon
            string semicolon = @"(\S*)(;)(.*)";
            Regex smicln = new Regex(semicolon);
            #endregion

            //make a list of spaces



            //special needed to allow the routine to run successfully
            //needed because the regex may return true in the wrong instance
            bool outsideBoundaryMatched = false;
            bool semicolonfound = false;
            bool detailedsurface = false;
            bool vertexMatching = false;
            foreach (string line in detailedSurfaceString)
            {

                #region
                MatchCollection surfaceStart = surfaceYes.Matches(line);
                if (surfaceStart.Count > 0)
                {
                    detailedsurface = true;
                    continue;
                }
                //now that a surface element is established in the IDF, we can work through it to create surface objects
                if (detailedsurface == true)
                {
                    //Surface Name
                    //get the name in the file
                    if (!vertexMatching)
                    {
                        Match surfaceNameMatch = surfaceNameRegex.Match(line);
                        int matchstart, matchlength = -1;
                        if (surfaceNameMatch.Success)
                        {
                            matchstart = surfaceNameMatch.Index;
                            matchlength = surfaceNameMatch.Length;
                            //strip off the whitespace and comma
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(surfaceNameMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                currentSurface.name = pure.Groups["goods"].Value;
                                continue;
                            }

                        }
                        //Get Surface Type
                        Match surfaceTypeMatch = surfaceTypeRegex.Match(line);
                        if (surfaceTypeMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(surfaceTypeMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                string type = pure.Groups["goods"].Value;
                                type = type.ToLower();
                                if (type == EPObj.EPSurface.SurfaceTypes.Ceiling.ToString().ToLower())
                                {
                                    currentSurface.surfaceType = EPObj.SurfaceTypes.Ceiling;
                                    continue;
                                }
                                else if (type == EPObj.EPSurface.SurfaceTypes.Floor.ToString().ToLower())
                                {
                                    currentSurface.surfaceType = EPObj.SurfaceTypes.Floor;
                                    continue;
                                }
                                else if (type == EPObj.EPSurface.SurfaceTypes.Roof.ToString().ToLower())
                                {
                                    currentSurface.surfaceType = EPObj.SurfaceTypes.Roof;
                                    continue;
                                }
                                else if (type == EPObj.EPSurface.SurfaceTypes.Wall.ToString().ToLower())
                                {
                                    currentSurface.surfaceType = EPObj.SurfaceTypes.Wall;
                                    continue;
                                }

                            }
                        }
                        //Get Construction Type
                        Match constructionTypeMatch = surfaceConstructionNameRegex.Match(line);
                        if (constructionTypeMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(constructionTypeMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                currentSurface.constructionName = pure.Groups["goods"].Value;
                                continue;
                            }

                        }
                        //GetZone Name
                        Match zoneNameMatch = surfaceZoneNameRegex.Match(line);
                        if (zoneNameMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(zoneNameMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                currentSurface.zoneName = pure.Groups["goods"].Value;
                                continue;
                            }
                        }
                        //GetOutside Boundary Name

                        if (!outsideBoundaryMatched)
                        {
                            Match outsideBoundaryMatch = outsideBoundaryRegex.Match(line);
                            if (outsideBoundaryMatch.Success)
                            #region
                            {
                                string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                                Regex purifyRegex = new Regex(purify);
                                Match pure = purifyRegex.Match(outsideBoundaryMatch.Groups["1"].Value);
                                if (pure.Success)
                                {

                                    string type = pure.Groups["goods"].Value;
                                    type = type.ToLower();
                                    if (type == EPObj.EPSurface.OutsideBoundary.Ground.ToString().ToLower())
                                    {
                                        currentSurface.outsideBoundary = EPObj.OutsideBoundary.Ground;
                                        outsideBoundaryMatched = true;
                                    }
                                    else if (type == EPObj.EPSurface.OutsideBoundary.OtherSideCoefficients.ToString().ToLower())
                                    {
                                        currentSurface.outsideBoundary = EPObj.OutsideBoundary.OtherSideCoefficients;
                                        outsideBoundaryMatched = true;
                                    }
                                    else if (type == EPObj.EPSurface.OutsideBoundary.OtherSideConditionsModel.ToString().ToLower())
                                    {
                                        currentSurface.outsideBoundary = EPObj.OutsideBoundary.OtherSideConditionsModel;
                                        outsideBoundaryMatched = true;
                                    }
                                    else if (type == EPObj.EPSurface.OutsideBoundary.Outdoors.ToString().ToLower())
                                    {
                                        currentSurface.outsideBoundary = EPObj.OutsideBoundary.Outdoors;
                                        outsideBoundaryMatched = true;
                                    }
                                    else if (type == EPObj.EPSurface.OutsideBoundary.Surface.ToString().ToLower())
                                    {
                                        currentSurface.outsideBoundary = EPObj.OutsideBoundary.Surface;
                                        outsideBoundaryMatched = true;
                                    }
                                    else
                                    {
                                        currentSurface.outsideBoundary = EPObj.OutsideBoundary.Zone;
                                        outsideBoundaryMatched = true;
                                    }
                                    continue;
                                }
                            }
                        }
                            #endregion

                        //Get Outside Boundary Condition Object
                        Match outsideBoundaryConditionMatch = outsideBoundaryConditionRegex.Match(line);
                        if (outsideBoundaryConditionMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(outsideBoundaryConditionMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                currentSurface.outsideBoundaryCondition = pure.Groups["goods"].Value;
                                continue;
                            }
                        }
                        //Get Sun Exposure
                        Match sunExposureMatch = sunExposureRegex.Match(line);
                        if (sunExposureMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(sunExposureMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                currentSurface.sunExposureVar = pure.Groups["goods"].Value;
                                continue;
                            }
                        }
                        //Get Wind Exposure
                        Match windExposureMatch = windExposureRegex.Match(line);
                        if (windExposureMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(windExposureMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                currentSurface.windExposureVar = pure.Groups["goods"].Value;
                                continue;
                            }
                        }
                        //View Factor
                        Match viewFactorMatch = viewFactorRegex.Match(line);
                        if (viewFactorMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(viewFactorMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                if (pure.Groups["goods"].Value == "AutoCalculate")
                                {
                                    currentSurface.viewFactor = -999;
                                }
                                else
                                {
                                    currentSurface.viewFactor = Convert.ToDouble(pure.Groups["goods"].Value);
                                }
                                continue;
                            }
                        }
                        //Number of Vertices
                        Match numVerticesMatch = numberofVerticesRegex.Match(line);
                        if (numVerticesMatch.Success)
                        {
                            string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma',)";
                            Regex purifyRegex = new Regex(purify);
                            Match pure = purifyRegex.Match(numVerticesMatch.Groups["1"].Value);
                            if (pure.Success)
                            {
                                currentSurface.numVertices = Convert.ToInt32(pure.Groups["goods"].Value);
                                continue;
                            }
                        }
                    }
                    //Get Vertices
                    //loop through them until the end


                    Match vertexMatch = typicalVertexRegex.Match(line);
                    if (vertexMatch.Success)
                    {
                        vertexMatching = true;
                        string purify = @"(?'ws'\s*)(?'goods'.*)(?'comma')";
                        Regex purifyRegex = new Regex(purify);
                        Match pure = purifyRegex.Match(vertexMatch.Groups["1"].Value);
                        if (pure.Success)
                        {
                            //extract the X,Y, and Z coordinate from the purified string
                            //string coordinateString = @"(?'X'[-]\d+[\.]\d+|\d+),(?'Y'[-]\d+[\.]\d+|\d+),(?'Z'[-]\d+[\.]\d+|\d+)";
                            string coordinateString = @"(?'X'[-+]?([0-9]*\.[0-9]+|[0-9]+)),\s*(?'Y'[-+]?([0-9]*\.[0-9]+|[0-9]+)),\s*(?'Z'[-+]?([0-9]*\.[0-9]+|[0-9]+)\s*)";
                            Regex coordRegex = new Regex(coordinateString);
                            Match XYZMatch = coordRegex.Match(pure.Groups["goods"].Value);
                            if (XYZMatch.Success)
                            {
                                EPObj.CartCoord surfaceCoord = new EPObj.CartCoord();
                                surfaceCoord.X = Convert.ToDouble(XYZMatch.Groups["X"].Value);
                                surfaceCoord.Y = Convert.ToDouble(XYZMatch.Groups["Y"].Value);
                                surfaceCoord.Z = Convert.ToDouble(XYZMatch.Groups["Z"].Value);
                                currentSurface.SurfaceCoords.Add(surfaceCoord);
                            }
                        }


                        //see if there is a semi-colon
                        Match smicolonMatch = smicln.Match(line);
                        if (smicolonMatch.Success)
                        {
                            semicolonfound = true;
                            vertexMatching = false;
                        }
                    }
                }
            }
                #endregion
            //close the reader


            //get the RHR Normal Vector
            EPObj.CartVect RHRNormalVector = EPObj.GetRHR(currentSurface.SurfaceCoords);
            logline.AppendLine(currentSurface.name + ", " + currentSurface.surfaceType + ", " + currentSurface.outsideBoundary.ToString());
            Console.WriteLine(currentSurface.name + ", " + currentSurface.surfaceType + ", " + currentSurface.outsideBoundary.ToString());
            //get azimuth
            currentSurface.azimuth = FindAzimuth(RHRNormalVector);
            //get tilt
            EPObj.MemorySafe_CartVect memRHR = EPObj.convertToMemorySafeVector(RHRNormalVector);
            currentSurface.tilt = FindTilt(memRHR);
            //in any case, return the current surface
            logline.AppendLine(RHRNormalVector.X.ToString() + ", " + RHRNormalVector.Y.ToString() + ", " + RHRNormalVector.Z.ToString() + ", " + currentSurface.azimuth.ToString() + ", " + currentSurface.tilt.ToString());
            Console.WriteLine(RHRNormalVector.X.ToString() + ", " + RHRNormalVector.Y.ToString() + ", " + RHRNormalVector.Z.ToString() + ", " + currentSurface.azimuth.ToString() + ", " + currentSurface.tilt.ToString());


            return currentSurface;
        }
    }
}
