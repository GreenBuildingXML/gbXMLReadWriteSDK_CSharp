using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;
using VectorMath;

namespace gbXMLSerializer
{
    

    public class BasicSerialization
    {
        public class LLRet
        {
            public CartesianPoint cp { get; set; }
            public List<int> indices { get; set; }
        }

        
        //needed  to store all unique planes in the project
        public static List<PlanarGeometry> uniquesurf = new List<PlanarGeometry>();
        public static Dictionary<string, PlanarGeometry> uniqueplanes = new Dictionary<string, PlanarGeometry>();
        public static Dictionary<string, Surface> uniquesurfaces = new Dictionary<string, Surface>();

        public static bool CreateSerial(string filepath)
        {
            //place an in memory object here that represents your class representation of the building
            List<EPObj.MemorySafe_Spaces> myspace = new List<EPObj.MemorySafe_Spaces>();
            myspace = EnergyPlusClass.EPlusSpacestoObjectList(@"C:\Users\Chiensi\Documents\C\Buro Happold\Oregon Sustainability Center\Run 1 + Daylighting Only\Run 1 new.idf");

            gb.gbci = new CultureInfo(String.Empty);

            bool ret = false;

            //the basics
            //constructor to define the basics
            gbXML gbx = new gbXML();
            gbx.lengthUnit = lengthUnitEnum.Feet;
            gbx.temperatureUnit = temperatureUnitEnum.F;
            string id = "cmps-1";
            Campus cmp = CreateCampus(id);
            cmp.Buildings = new Building[10000];
            gbx.Campus = cmp;

            //where does this location information from?  it could be smartly inferred somehow, but otherwise specified by the user/programmer
            Location zeloc = new Location();
            zeloc.Name = "San Francisco, CA";
            zeloc.Latitude = "37.795";
            zeloc.Longitude = "-122.394";
            //end the basics
            //tie location and campus back to the gbXML file
            cmp.Location = zeloc;

            //Define the building(s) on the site
            //CHarriman Septempber 19 2013
            cmp.Buildings[0] = MakeBuilding(2000,"bldg-1",buildingTypeEnum.DiningBarLoungeOrLeisure);

            //CHarriman September 19 2013
            //define the stories for each building
            //several ways to do this
            List<List<double>> points = prod.MakeFakeList(5);
            BuildingStorey bs = MakeStorey(1, points);

            //CHarriman Jan 15 2014
            //define the spaces for each building (these come from a space object elsewhere
            List<Space> gbSpaces = new List<Space>();
            gbSpaces = MakeSpacesFromEPObj(myspace);

            
            for (int spacecount = 0; spacecount < gbSpaces.Count(); spacecount++)
            {
                cmp.Buildings[0].Spaces[spacecount] = gbSpaces[spacecount];
            }
                

            //after making all the spaces, I make the surfaces
            cmp.Surface = new Surface[uniquesurfaces.Count()];
            int surfcount = 0;
            foreach (KeyValuePair<string, Surface> pair in uniquesurfaces)
            {
                Surface surf = new Surface();
                surf.id = pair.Key;
                //this is a hard one, how to deal with this?  For now, everything is external, and idf can sort of tell me
                surf.surfaceType = pair.Value.surfaceType;
                surf.constructionIdRef = pair.Value.constructionIdRef;
                surf.Name = pair.Value.Name;
                AdjacentSpaceId[] adjspaces = new AdjacentSpaceId[pair.Value.AdjacentSpaceId.Count()];
                int counter = 0;
                foreach (AdjacentSpaceId adj in pair.Value.AdjacentSpaceId)
                {
                    adjspaces[counter] = adj;
                    counter++;
                }
                surf.AdjacentSpaceId = adjspaces;
                RectangularGeometry rg = new RectangularGeometry();
                rg = pair.Value.RectangularGeometry;
                surf.RectangularGeometry = rg;
                surf.PlanarGeometry = pair.Value.PlanarGeometry;
                cmp.Surface[surfcount] = surf;
                surfcount++;
            }


            cmp.Buildings[0].bldgStories[0] = bs;

            //write xml to the file
            XmlSerializer szer = new XmlSerializer(typeof(gbXML));
            TextWriter tw = new StreamWriter(filepath);
            szer.Serialize(tw, gbx);
            tw.Close();

            return ret;
        }

        public static List<Space> MakeSpacesFromEPObj(List<EPObj.MemorySafe_Spaces> myspace)
        {
            List<Space> retspaces = new List<Space>();
            int spacecount = 0;
            foreach (EPObj.MemorySafe_Spaces space in myspace)
            {
                //foreach Space space in your ListofSpaces
                Space zespace = new Space();
                zespace.id = "Space-1";
                zespace.lightScheduleIdRef = "lightSchedule-1";
                zespace.equipmentScheduleIdRef = "equipmentSchedule-1";
                zespace.peopleScheduleIdRef = "peopleSchedule-1";
                zespace.conditionType = "HeatedAndCooled";
                zespace.buildingStoreyIdRef = "bldg-story-1";
                zespace.Name = "Test Space-" + spacecount;
                zespace.peoplenum = 12;
                zespace.totalpeoplegain = 450;
                zespace.senspeoplegain = 250;
                zespace.latpeoplegain = 200;
                zespace.PeopleHeatGains = new PeopleHeatGain[3];
                zespace.lpd = 1.2;
                zespace.epd = 1.5;
                zespace.Area = 2450;
                zespace.Volume = 24500;
                zespace.PlanarGeo = new PlanarGeometry();
                zespace.ShellGeo = new ShellGeometry();


                PeopleNumber pn = new PeopleNumber();
                pn.unit = peopleNumberUnitEnum.NumberOfPeople;

                string people = gb.FormatDoubleToString(zespace.peoplenum);
                pn.valuefield = people;
                zespace.PeopleNumber = pn;

                PeopleHeatGain phg = new PeopleHeatGain();
                phg.unit = peopleHeatGainUnitEnum.BtuPerHourPerson;
                phg.heatGainType = peopleHeatGainTypeEnum.Total;
                string totalpopload = gb.FormatDoubleToString(zespace.totalpeoplegain);
                phg.value = totalpopload;
                zespace.PeopleHeatGains[0] = phg;

                PeopleHeatGain shg = new PeopleHeatGain();
                shg.unit = peopleHeatGainUnitEnum.BtuPerHourPerson;
                shg.heatGainType = peopleHeatGainTypeEnum.Sensible;
                string senspopload = gb.FormatDoubleToString(zespace.senspeoplegain);
                shg.value = senspopload;
                zespace.PeopleHeatGains[1] = shg;

                PeopleHeatGain lhg = new PeopleHeatGain();
                lhg.unit = peopleHeatGainUnitEnum.BtuPerHourPerson;
                lhg.heatGainType = peopleHeatGainTypeEnum.Latent;
                string latpopload = gb.FormatDoubleToString(zespace.latpeoplegain);
                lhg.value = latpopload;
                zespace.PeopleHeatGains[2] = lhg;

                LightPowerPerArea lpd = new LightPowerPerArea();
                lpd.unit = powerPerAreaUnitEnum.WattPerSquareFoot;
                lpd.lpd = gb.FormatDoubleToString(zespace.lpd);
                zespace.LightPowerPerArea = lpd;

                EquipPowerPerArea epd = new EquipPowerPerArea();
                epd.unit = powerPerAreaUnitEnum.WattPerSquareFoot;
                epd.epd = gb.FormatDoubleToString(zespace.epd);
                zespace.EquipPowerPerArea = epd;

                Area spacearea = new Area();
                spacearea.val = gb.FormatDoubleToString(zespace.Area);
                zespace.spacearea = spacearea;

                Volume spacevol = new Volume();
                spacevol.val = gb.FormatDoubleToString(zespace.Volume);
                zespace.spacevol = spacevol;

                //same as the planar geometry of the floor planes above
                PlanarGeometry spaceplpoly = new PlanarGeometry();
                //get a list of points that makes up the polyloop
                List<List<double>> spacepoints = prod.MakeFakeList(3);
                //make polyloop with points
                spaceplpoly.PolyLoop = new PolyLoop();
                spaceplpoly.PolyLoop = prod.makePolyLoopsFromDbleList(spaceplpoly.PolyLoop, spacepoints);
                zespace.PlanarGeo = spaceplpoly;
                //@@
                //ShellGeometry
                //similar to planar geometry, but with more planes
                ShellGeometry sg = new ShellGeometry();
                sg.unit = lengthUnitEnum.Feet;
                sg.id = "sg" + space.name;
                sg.ClosedShell = new ClosedShell();
                //up to 100 surfaces per space?  base on the space instance surfaces
                sg.ClosedShell.PolyLoops = new PolyLoop[space.spaceSurfaces.Count()];
                //I would have a list of surface elements that make up the space surfaces.
                //each surface would consist of a series of points that defines the surface.
                for (int i = 0; i < space.spaceSurfaces.Count(); i++)
                {
                    //get points from the space surfaces
                    List<List<double>> epluspoints = new List<List<double>>();
                    epluspoints = EnergyPlusClass.GetCoordinDoubles(space.spaceSurfaces[i].SurfaceCoords);
                    sg.ClosedShell.PolyLoops[i] = new PolyLoop();
                    sg.ClosedShell.PolyLoops[i] = prod.makePolyLoopsFromDbleList(sg.ClosedShell.PolyLoops[i], epluspoints);
                }
                zespace.ShellGeo = sg;

                zespace.cadid = new CADObjectId();
                zespace.cadid.id = "990099-" + spacecount;
                //make surface boundaries..special code needed so that space boundaries are not duplicated...
                //option 1 : the surfaces already declared as internal somehow and how shared.
                //option 2:  the api tries to figure it out
                zespace.spbound = new SpaceBoundary[space.spaceSurfaces.Count()];
                int psurfacecount = 0;
                for (int i = 0; i < space.spaceSurfaces.Count(); i++)
                {
                    //get points from the space surfaces
                    List<List<double>> epluspoints = new List<List<double>>();
                    epluspoints = EnergyPlusClass.GetCoordinDoubles(space.spaceSurfaces[i].SurfaceCoords);
                    //if surface is exterior
                    SpaceBoundary sb = new SpaceBoundary();
                    zespace.spbound[i] = prod.MakeSpaceBoundary(sb, epluspoints, psurfacecount);

                    psurfacecount++;
                    //else if surface is interior and it has not been listed before
                    //then do the same
                    //else do nothing because it is interior and it has already been listed


                    //I also would like to keep track of all the surfaces that I create to better prepare me for the surface definition
                    uniquesurf.Add(zespace.spbound[i].PlanarGeometry);
                    //make a dictionary that stores the name of a surface and its planar geometry?
                    uniqueplanes.Add(zespace.spbound[i].surfaceIdRef, zespace.spbound[i].PlanarGeometry);
                    //make a dictionary that stores the name of a surface and create a surface as the value

                    Surface newsurface = new Surface();
                    //this took a lot of customization...a user would have to make their own code to attach to my object here
                    newsurface = prod.SetUpSurfaceFromIDF(space.spaceSurfaces[i], zespace.spbound[i].PlanarGeometry);
                    uniquesurfaces.Add(zespace.spbound[i].surfaceIdRef, newsurface);
                }


                retspaces.Add(zespace);
                spacecount++;
            }
            return retspaces;
        }

        public static Space assignSimpleDefaults(Space zespace, int zonecount)
        {
            CultureInfo ci = new CultureInfo(String.Empty);
            
            try
            {
                //area and volume should already be computed
                //zespace.id = "Space-1";
                zespace.lightScheduleIdRef = "lightSchedule-1";
                zespace.equipmentScheduleIdRef = "equipmentSchedule-1";
                zespace.peopleScheduleIdRef = "peopleSchedule-1";
                zespace.conditionType = "HeatedAndCooled";
                zespace.buildingStoreyIdRef = "bldg-story-1";
                zespace.Name = "Test Space-" + zonecount;

                zespace.peoplenum = zespace.Area / 150;
                PeopleNumber pn = new PeopleNumber();
                pn.unit = peopleNumberUnitEnum.NumberOfPeople;
                pn.valuefield = (zespace.Area/150).ToString();
                zespace.PeopleNumber = pn;

                zespace.PeopleHeatGains = new PeopleHeatGain[3];

                zespace.totalpeoplegain = 450;
                PeopleHeatGain tph = new PeopleHeatGain();
                tph.heatGainType = peopleHeatGainTypeEnum.Total;
                tph.unit = peopleHeatGainUnitEnum.BtuPerHourPerson;
                tph.value = string.Format(ci, "{0:0.000000}","450");
                zespace.PeopleHeatGains[0] = tph;

                zespace.senspeoplegain = 250;
                PeopleHeatGain sph = new PeopleHeatGain();
                sph.heatGainType = peopleHeatGainTypeEnum.Sensible;
                sph.unit = peopleHeatGainUnitEnum.BtuPerHourPerson;
                sph.value = string.Format(ci, "{0:0.000000}","250");
                zespace.PeopleHeatGains[1] = sph;

                zespace.latpeoplegain = 200;
                PeopleHeatGain lph = new PeopleHeatGain();
                lph.heatGainType = peopleHeatGainTypeEnum.Latent;
                lph.unit = peopleHeatGainUnitEnum.BtuPerHourPerson;
                lph.value = string.Format(ci, "{0:0.000000}","200");
                zespace.PeopleHeatGains[2] = lph;

                zespace.lpd = 1.2;
                LightPowerPerArea lp = new LightPowerPerArea();
                lp.unit = powerPerAreaUnitEnum.WattPerSquareFoot;
                lp.lpd = string.Format(ci, "{0:0.000000}","1.2");
                zespace.LightPowerPerArea = lp;

                zespace.epd = 1.5;
                EquipPowerPerArea ep = new EquipPowerPerArea();
                ep.unit = powerPerAreaUnitEnum.WattPerSquareFoot;
                ep.epd = string.Format(ci, "{0:0.000000}","1.5");
                zespace.EquipPowerPerArea = ep;

                Area a = new Area();
                a.val = string.Format(ci, "{0:0.000000}",zespace.Area.ToString());
                zespace.spacearea = a;

                Volume v = new Volume();
                v.val = string.Format(ci, "{0:0.000000}",zespace.Volume.ToString());
                zespace.spacevol = v;

                
                //everything is prepared for geometry
                zespace.PlanarGeo = new PlanarGeometry();
                zespace.ShellGeo = new ShellGeometry();
                return zespace;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Campus CreateCampus(string id)
        {
            Campus cmp = new Campus();
            cmp.id = id;
            return cmp;

        }

        public static Building[] SetBldArray(int buildnum)
        {
            return new Building[buildnum];
        }

        public static SpaceBoundary[] makeSBArray(int size)
        {
            return new SpaceBoundary[size];
        }

        public static PeopleHeatGain[] makePeopleHeatGainAray(int size)
        {
            return new PeopleHeatGain[size];
        }

        public static Absorptance[] makeAbsorptanceArray(int size)
        {
            return new Absorptance[size];
        }

        public static Emittance[] makeEmittanceArray(int size)
        {
            return new Emittance[size];
        }

        public static string[] makeCoordinatesArray(int size)
        {
            return new string[size];
        }

        public static CartesianPoint[] makeCartesianPtArray(int size)
        {
            return new CartesianPoint[size];
        }

        public static BuildingStorey[] setLevelsArray(int size)
        {
            return new BuildingStorey[size];
        }

        public static ScheduleValue[] setScheduleValuesArray(int size)
        {
            return new ScheduleValue[size];
        }

        public static Schedule[] setScheduleArray(int size)
        {
            return new Schedule[size];
        }

        public static WeekSchedule[] setWeekScheduleArray(int size)
        {
            return new WeekSchedule[size];
        }

        public static DaySchedule[] setDayScheduleArray(int size)
        {
            return new DaySchedule[size];
        }

        public static Day[] setDayArray(int size)
        {
            return new Day[size];
        }

        public static YearSchedule[] setYearScheduleArray(int size)
        {
            return new YearSchedule[size];
        }

        public static Surface[] defSurfaceArray(int size)
        {
            return new Surface[size];
        }

        public static AdjacentSpaceId[] defAdjSpID(int size)
        {
            return new AdjacentSpaceId[size];
        }

        public static Opening[] defOpeningsArr(int size)
        {
            return new Opening[size];
        }

        public static Material[] defMaterialsArray(int size)
        {
            return new Material[size];
        }

        public static Construction[] defConstructionArray(int size)
        {
            return new Construction[size];
        }

        public static Layer[] defLayerArray(int size)
        {
            return new Layer[size];
        }

        public static LayerId[] defLayerIdArray(int size)
        {
            return new LayerId[size];
        }

        public static MaterialId[] defMaterialIdArray(int size)
        {
            return new MaterialId[size];
        }

        public static Cost[] defCostArray(int size)
        {
            return new Cost[size];
        }

        public static WindowType[] defWindowTypeArray(int size)
        {
            return new WindowType[size];
        }

        public static Transmittance[] defTransmittanceArray(int size)
        {
            return new Transmittance[size];

        }

        public static Reflectance[] defReflectanceArray(int size)
        {
            return new Reflectance[size];
        }

        public static Emittance[] defEmmitanceArray(int size)
        {
            return new Emittance[size];
        }

        public static Glaze[] defGlazeArray(int size)
        {
            return new Glaze[size];
        }

        public static SolarHeatGainCoeff[] defSolarHeatGainArray(int size)
        {
            return new SolarHeatGainCoeff[size];
        }

        public static Gap[] defGapArray(int size)
        {
            return new Gap[size];
        }
        public static Building MakeBuilding(double bldarea, string bldgname,buildingTypeEnum bldgType)
        {
            Building zeb = new Building();
            zeb.Area = bldarea;
            zeb.id = bldgname;
            zeb.buildingType = bldgType;
            //this has been arbitrarily defined and could be changed
            zeb.bldgStories = new BuildingStorey[1000];
            zeb.Spaces = new Space[10000];
            return zeb;
        }

        public static List<Building> MakeBuildings(List<double> bldareas,List<buildingTypeEnum> types)
        {
            List<Building> blds = new List<Building>();
            int bldgcount = 1;
            for (int i=0; i < bldareas.Count(); i++)
            {
                string bldgid = "bldg-"+bldgcount;
                var area = bldareas[i];
                var type = types[i];
                blds.Add(MakeBuilding(area,bldgid,type));
            }
            return blds;
        }

        public static BuildingStorey MakeStorey(int levelnum, List<List<double>> points)
        {
            BuildingStorey bs = new BuildingStorey();
            bs.id = "bldg-story-" + levelnum;
            bs.Name = "Level " + levelnum;
            bs.Level = (levelnum - 1).ToString();

            //there is only one plane per storey
            PlanarGeometry pg = new PlanarGeometry();
            //make polyloop with points
            pg.PolyLoop = new PolyLoop();
            //uses just a simple set of points = List<List<double>>
            pg.PolyLoop = prod.makePolyLoopsFromDbleList(pg.PolyLoop, points);
            //add polyloop to the building storey object
            bs.PlanarGeo = pg;
            return bs;
        }

        //same but from a series of Vector Cartesian Coordinates
        public static BuildingStorey MakeStorey(int levelnum, List<Vector.MemorySafe_CartCoord> points)
        {
            BuildingStorey bs = new BuildingStorey();
            bs.id = "bldg-story-" + levelnum;
            bs.Name = "Level " + levelnum;
            bs.Level = (levelnum - 1).ToString();

            //there is only one plane per storey
            PlanarGeometry pg = new PlanarGeometry();
            //make polyloop with points
            pg.PolyLoop = new PolyLoop();
            //uses just a simple set of points = List<List<double>>
            pg.PolyLoop = prod.MakePolyLoops(pg.PolyLoop, points);
            //add polyloop to the building storey object
            bs.PlanarGeo = pg;
            return bs;
        }

        public static PlanarGeometry makegbPlanarGeom(List<List<Vector.MemorySafe_CartCoord>> coordinates)
        {
            PlanarGeometry pg = new PlanarGeometry();
            PolyLoop pl = new PolyLoop();
            pg.PolyLoop = pl;
            for (int i = 0; i < coordinates.Count(); i++)
            {
                //the polyloop array of points is defined
                pl.Points = new CartesianPoint[coordinates[i].Count()];
                for (int j = 0; j < coordinates[i].Count(); j++)
                {
                    //returns a point with three coordinate strings
                    CartesianPoint cp = makegbCartesianPt(coordinates[i][j]);
                    //the point is added to the polyloop
                    pl.Points[j] = cp;
                }
            }
            return pg;
        }

        public static CartesianPoint makegbCartesianPt(Vector.MemorySafe_CartCoord pt)
        {
            CartesianPoint cp = new CartesianPoint();
            cp.Coordinate = new string[3];

            cp.Coordinate[0] = gb.FormatDoubleToString(pt.X);
            cp.Coordinate[1] = gb.FormatDoubleToString(pt.Y);
            cp.Coordinate[2] = gb.FormatDoubleToString(pt.Z);
            return cp;
        }

        public static List<BuildingStorey> MakeStories(List<List<Vector.MemorySafe_CartCoord>> pointslist)
        {
            List<BuildingStorey> stories = new List<BuildingStorey>();
            for (int i = 1; i <= pointslist.Count(); i++)
            {
                BuildingStorey bs = new BuildingStorey();
                bs.id = "bldg-story-" + i;
                bs.Name = "Level " + i;
                bs.Level = (i - 1).ToString();

                //there is only one plane per storey
                PlanarGeometry pg = new PlanarGeometry();
                //make polyloop with points
                pg.PolyLoop = new PolyLoop();
                //uses just a simple set of points = List<List<double>>
                pg.PolyLoop = prod.MakePolyLoops(pg.PolyLoop, pointslist[i]);
                //add polyloop to the building storey object
                bs.PlanarGeo = pg;
                stories.Add(bs);
            }
            return stories;
        }

        public static string CreateXML(string filename, gbXML gbx)
        {
            try
            {
                XmlSerializer szer = new XmlSerializer(typeof(gbXML));
                TextWriter tw = new StreamWriter(filename);
                szer.Serialize(tw, gbx);
                tw.Close();
                return "Ok";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static LLRet GetLLForFloor(List<Vector.MemorySafe_CartCoord> surfacecoords)
        {
            LLRet ll = new LLRet();
            ll.indices = new List<int>();
            int surfindex = -1;
            CartesianPoint cp = new CartesianPoint();

            Vector.CartCoord llsurf = new Vector.CartCoord();

            Vector.MemorySafe_CartVect RHRVector = Vector.GetMemRHR(surfacecoords);
            if (Math.Abs(RHRVector.X) == 0 && RHRVector.Y == 0 && RHRVector.Z == -1)
            {
                
                for (int sccount = 0; sccount < surfacecoords.Count; sccount++)
                {
                    if (sccount == 0)
                    {
                        llsurf.X = surfacecoords[sccount].X;
                        llsurf.Y = surfacecoords[sccount].Y;
                        llsurf.Z = surfacecoords[sccount].Z;
                        surfindex = sccount;
                        continue;
                    }
                    //get lower left...most low(smallest Y), then most left (largest X)
                    if (surfacecoords[sccount].Y <= llsurf.Y)
                    {
                        if (surfacecoords[sccount].X > llsurf.X)
                        {
                            llsurf.X = surfacecoords[sccount].X;
                            llsurf.Y = surfacecoords[sccount].Y;
                            llsurf.Z = surfacecoords[sccount].Z;
                            surfindex = sccount;
                        }
                    }
                }
            }
            else
            {
                //special procedure for this type of floor
            }

            Vector.MemorySafe_CartCoord LLeft = new Vector.MemorySafe_CartCoord(llsurf.X, llsurf.Y, llsurf.Z);
            cp = makegbCartesianPt(LLeft);

            ll.cp = cp;
            ll.indices.Add(surfindex);
            return ll;
        }

        public static LLRet GetLLForRoof(List<Vector.MemorySafe_CartCoord> surfacecoords)
        {
            LLRet ll = new LLRet();
            ll.indices = new List<int>();
            int surfindex = -1;
            CartesianPoint cp = new CartesianPoint();

            Vector.CartCoord llsurf = new Vector.CartCoord();

            Vector.MemorySafe_CartVect RHRVector = Vector.GetMemRHR(surfacecoords);
            if (Math.Abs(RHRVector.X) == 0 && RHRVector.Y == 0 && RHRVector.Z == 1)
            {
                
                for (int sccount = 0; sccount < surfacecoords.Count; sccount++)
                {
                    if (sccount == 0)
                    {
                        llsurf.X = surfacecoords[sccount].X;
                        llsurf.Y = surfacecoords[sccount].Y;
                        llsurf.Z = surfacecoords[sccount].Z;
                        surfindex = sccount;
                        continue;

                    }
                    //get lower left...most low(largest Y), then most left (smallest X)
                    if (surfacecoords[sccount].Y >= llsurf.Y)
                    {
                        if (surfacecoords[sccount].X < llsurf.X)
                        {
                            llsurf.X = surfacecoords[sccount].X;
                            llsurf.Y = surfacecoords[sccount].Y;
                            llsurf.Z = surfacecoords[sccount].Z;
                            surfindex = sccount;
                        }
                    }
                }
            }
            else
            {
                //special procedure for this type of roof
            }
            Vector.MemorySafe_CartCoord LLeft = new Vector.MemorySafe_CartCoord(llsurf.X, llsurf.Y, llsurf.Z);
            cp = makegbCartesianPt(LLeft);
            ll.cp = cp;
            ll.indices.Add(surfindex);
            return ll;
        }
        
        //Reminder to factor in CADAzimuth if appropriate
        public static LLRet GetLLForOpening(List<Vector.MemorySafe_CartCoord> surfacecoords, List<Vector.MemorySafe_CartCoord> openingcoords)
        {

            LLRet ll = new LLRet();
            ll.indices = new List<int>();
            int surfindex=-1;
            int opindex=-1;
            CartesianPoint cp = new CartesianPoint();
            
            //we want to verify that this is indeed correct
            //west-facing

            Vector.MemorySafe_CartVect RHRVector = Vector.GetMemRHR(surfacecoords);
            if (Math.Abs(RHRVector.X) == -1 && RHRVector.Y == 0 && RHRVector.Z == 0)
            {

                Vector.CartCoord llsurf = new Vector.CartCoord();
                for (int sccount = 0; sccount < surfacecoords.Count; sccount++)
                {
                    if (sccount == 0)
                    {
                        llsurf.X = surfacecoords[sccount].X;
                        llsurf.Y = surfacecoords[sccount].Y;
                        llsurf.Z = surfacecoords[sccount].Z;
                        surfindex = sccount;
                        continue;
                    }
                    //get lower left...most lowest(smallest Z), then most left (largest Y)
                    if (surfacecoords[sccount].Z <= llsurf.Z)
                    {
                        if(surfacecoords[sccount].Y > llsurf.Y)
                        {
                            llsurf.X = surfacecoords[sccount].X;
                            llsurf.Y = surfacecoords[sccount].Y;
                            llsurf.Z = surfacecoords[sccount].Z;
                            surfindex = sccount;
                        }
                    }
                }

                Vector.CartCoord llopening = new Vector.CartCoord();
                for (int occount = 0; occount < openingcoords.Count; occount++)
                {
                    if (occount == 0)
                    {
                        llopening.X = openingcoords[occount].X;
                        llopening.Y = openingcoords[occount].Y;
                        llopening.Z = openingcoords[occount].Z;
                        opindex = occount;
                        continue;
                    }
                    //get lower left...most low(lowest Z), then most left (largest Y)
                    if (openingcoords[occount].Z <= llopening.Z)
                    {
                        if(openingcoords[occount].Y > llopening.Y)
                        {
                            llopening.X = openingcoords[occount].X;
                            llopening.Y = openingcoords[occount].Y;
                            llopening.Z = openingcoords[occount].Z;
                            opindex = occount;
                        }
                    }
                }
                double diffX = Math.Abs(llsurf.Y - llopening.Y);
                double diffY = Math.Abs(llopening.Z - llsurf.Z);
                Vector.MemorySafe_CartCoord LLeft = new Vector.MemorySafe_CartCoord(diffX,diffY,0);
                cp = makegbCartesianPt(LLeft);
            }
            //north-facing
            else if (Math.Abs(RHRVector.X) == 0 && RHRVector.Y == 1 && RHRVector.Z == 0)
            {

                Vector.CartCoord llsurf = new Vector.CartCoord();
                for (int sccount = 0; sccount < surfacecoords.Count; sccount++)
                {
                    if (sccount == 0)
                    {
                        llsurf.X = surfacecoords[sccount].X;
                        llsurf.Y = surfacecoords[sccount].Y;
                        llsurf.Z = surfacecoords[sccount].Z;
                        surfindex = sccount;
                        continue;
                    }
                    //get lower left...most low(smallest Z), then most left (largest X)
                    if (surfacecoords[sccount].Z <= llsurf.Z)
                    {
                        if(surfacecoords[sccount].X > llsurf.X)
                        {
                            llsurf.X = surfacecoords[sccount].X;
                            llsurf.Y = surfacecoords[sccount].Y;
                            llsurf.Z = surfacecoords[sccount].Z;
                            surfindex = sccount;
                        }
                    }
                }

                Vector.CartCoord llopening = new Vector.CartCoord();
                for (int occount = 0; occount < openingcoords.Count; occount++)
                {
                    if (occount == 0)
                    {
                        llopening.X = openingcoords[occount].X;
                        llopening.Y = openingcoords[occount].Y;
                        llopening.Z = openingcoords[occount].Z;
                        opindex = occount;
                        continue;
                    }
                    //get lower left...most low(smallest Z), then most left (largest X)
                    if (openingcoords[occount].Z <= llopening.Z)
                    {
                        if(openingcoords[occount].X > llopening.X)
                        {
                            llopening.X = openingcoords[occount].X;
                            llopening.Y = openingcoords[occount].Y;
                            llopening.Z = openingcoords[occount].Z;
                            opindex = occount;
                        }
                    }
                }
                double diffX = Math.Abs(llsurf.X - llopening.X);
                double diffY = Math.Abs(llsurf.Z - llopening.Z);
                Vector.MemorySafe_CartCoord LLeft = new Vector.MemorySafe_CartCoord(diffX,diffY,0);
                cp= makegbCartesianPt(LLeft);
            }
            //south-facing
            else if (Math.Abs(RHRVector.X) == 0 && RHRVector.Y == -1 && RHRVector.Z == 0)
            {
                Vector.CartCoord llsurf = new Vector.CartCoord();
                for (int sccount = 0; sccount < surfacecoords.Count; sccount++)
                {
                    if (sccount == 0)
                    {
                        llsurf.X = surfacecoords[sccount].X;
                        llsurf.Y = surfacecoords[sccount].Y;
                        llsurf.Z = surfacecoords[sccount].Z;
                        surfindex = sccount;
                        continue;
                    }
                    //get lower left...most low(smaller Z), then most left (smallest X)
                    if (surfacecoords[sccount].Z <= llsurf.Z)
                    {
                        if(surfacecoords[sccount].X < llsurf.X)
                        {
                            llsurf.X = surfacecoords[sccount].X;
                            llsurf.Y = surfacecoords[sccount].Y;
                            llsurf.Z = surfacecoords[sccount].Z;
                            surfindex = sccount;
                        }
                    }
                }

                Vector.CartCoord llopening = new Vector.CartCoord();
                for (int occount = 0; occount < openingcoords.Count; occount++)
                {
                    if (occount == 0)
                    {
                        llopening.X = openingcoords[occount].X;
                        llopening.Y = openingcoords[occount].Y;
                        llopening.Z = openingcoords[occount].Z;
                        opindex = occount;
                        continue;
                    }
                    //get lower left...most low(smallest Z), then most left (smallest X)
                    if (openingcoords[occount].Z <= llopening.Z)
                    {
                        if(openingcoords[occount].X < llopening.X)
                        {
                            llopening.X = openingcoords[occount].X;
                            llopening.Y = openingcoords[occount].Y;
                            llopening.Z = openingcoords[occount].Z;
                            opindex = occount;
                        }
                    }
                }
                double diffX = Math.Abs(llsurf.X - llopening.X);
                double diffY = Math.Abs(llsurf.Z - llopening.Z);
                Vector.MemorySafe_CartCoord LLeft = new Vector.MemorySafe_CartCoord(diffX,diffY,0);
                cp = makegbCartesianPt(LLeft);
            }

            //east-facing
            else if (Math.Abs(RHRVector.X) == 1 && RHRVector.Y == 0 && RHRVector.Z == 0)
            {
                Vector.CartCoord llsurf = new Vector.CartCoord();
                for (int sccount = 0; sccount < surfacecoords.Count; sccount++)
                {
                    if (sccount == 0)
                    {
                        llsurf.X = surfacecoords[sccount].X;
                        llsurf.Y = surfacecoords[sccount].Y;
                        llsurf.Z = surfacecoords[sccount].Z;
                        surfindex = sccount;
                        continue;
                    }
                    //get lower left...most low(smaller Z), then most left (smallest Y)
                    if (surfacecoords[sccount].Z <= llsurf.Z)
                    {
                        if(surfacecoords[sccount].Y < llsurf.Y)
                        {
                            llsurf.X = surfacecoords[sccount].X;
                            llsurf.Y = surfacecoords[sccount].Y;
                            llsurf.Z = surfacecoords[sccount].Z;
                            surfindex = sccount;
                        }
                    }
                }

                Vector.CartCoord llopening = new Vector.CartCoord();
                for (int occount = 0; occount < openingcoords.Count; occount++)
                {
                    if (occount == 0)
                    {
                        llopening.X = openingcoords[occount].X;
                        llopening.Y = openingcoords[occount].Y;
                        llopening.Z = openingcoords[occount].Z;
                        opindex = occount;
                        continue;
                    }
                    //get lower left...most low(smallest Z), then most left (smallest Y)
                    if (openingcoords[occount].Z <= llopening.Z)
                    {
                        if(openingcoords[occount].Y < llopening.Y)
                        {
                            llopening.X = openingcoords[occount].X;
                            llopening.Y = openingcoords[occount].Y;
                            llopening.Z = openingcoords[occount].Z;
                            opindex = occount;
                        }
                    }
                }
                double diffX = Math.Abs(llsurf.Y - llopening.Y);
                double diffY = Math.Abs(llsurf.Z - llopening.Z);
                Vector.MemorySafe_CartCoord LLeft = new Vector.MemorySafe_CartCoord(diffX,diffY,0);
                cp = makegbCartesianPt(LLeft);
            }

            //floors
            else if (Math.Abs(RHRVector.X) == 0 && RHRVector.Y == 0 && RHRVector.Z == -1)
            {

                Vector.CartCoord llsurf = new Vector.CartCoord();
                LLRet llroof = GetLLForFloor(surfacecoords);
                llsurf.X = Convert.ToDouble(llroof.cp.Coordinate[0]);
                llsurf.Y = Convert.ToDouble(llroof.cp.Coordinate[1]);
                llsurf.Z = Convert.ToDouble(llroof.cp.Coordinate[2]);
                surfindex = llroof.indices[0];

                Vector.CartCoord llopening = new Vector.CartCoord();
                for (int occount = 0; occount < openingcoords.Count; occount++)
                {
                    if (occount == 0)
                    {
                        llopening.X = openingcoords[occount].X;
                        llopening.Y = openingcoords[occount].Y;
                        llopening.Z = openingcoords[occount].Z;
                        opindex = occount;

                        continue;
                    }
                    //get lower left...most low(smallest Y), then most left (largest X)
                    if (openingcoords[occount].Y <= llopening.Y)
                    {
                        if (openingcoords[occount].X > llopening.X)
                        {
                            llopening.X = openingcoords[occount].X;
                            llopening.Y = openingcoords[occount].Y;
                            llopening.Z = openingcoords[occount].Z;
                            opindex = occount;

                        }
                    }
                }
                double diffX = Math.Abs(llsurf.X - llopening.X);
                double diffY = Math.Abs(llsurf.Y - llopening.Y);
                Vector.MemorySafe_CartCoord LLeft = new Vector.MemorySafe_CartCoord(diffX, diffY, 0);
                cp = makegbCartesianPt(LLeft);
            }
            //flat roof
            else if (Math.Abs(RHRVector.X) == 0 && RHRVector.Y == 0 && RHRVector.Z == 1)
            {
                Vector.CartCoord llsurf = new Vector.CartCoord();
                LLRet llroof = GetLLForRoof(surfacecoords);
                llsurf.X = Convert.ToDouble(llroof.cp.Coordinate[0]);
                llsurf.Y = Convert.ToDouble(llroof.cp.Coordinate[1]);
                llsurf.Z = Convert.ToDouble(llroof.cp.Coordinate[2]);
                surfindex = llroof.indices[0];

                Vector.CartCoord llopening = new Vector.CartCoord();
                for (int occount = 0; occount < openingcoords.Count; occount++)
                {
                    if (occount == 0)
                    {
                        llopening.X = openingcoords[occount].X;
                        llopening.Y = openingcoords[occount].Y;
                        llopening.Z = openingcoords[occount].Z;
                        opindex = occount;
                        continue;
                    }
                    //get lower left...most low(largest Y), then most left (smallest X)
                    if (openingcoords[occount].Y >= llopening.Y)
                    {
                        if (openingcoords[occount].X < llopening.X)
                        {
                            llopening.X = openingcoords[occount].X;
                            llopening.Y = openingcoords[occount].Y;
                            llopening.Z = openingcoords[occount].Z;
                            opindex = occount;
                                
                        }
                    }
                }
                double diffX = Math.Abs(llsurf.X - llopening.X);
                double diffY = Math.Abs(llsurf.Y - llopening.Y);
                Vector.MemorySafe_CartCoord LLeft = new Vector.MemorySafe_CartCoord(diffX, diffY, 0);
                cp = makegbCartesianPt(LLeft);
            }
            //plane does not reside on a primary axis 
            else
            {
                //I will deal with this later
                //have to factor in what is a "roof" or "floor" or "wall" based on the default tilt settings
            }
            ll.cp = cp;
            ll.indices.Add(surfindex);
            ll.indices.Add(opindex);
            return ll;
        }
    }
}