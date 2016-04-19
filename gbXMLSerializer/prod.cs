using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using VectorMath;

namespace gbXMLSerializer
{
    public class prod
    {

        static public double FindTilt(Vector.MemorySafe_CartVect normalVector)
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
            Vector.MemorySafe_CartVect northVector = new Vector.MemorySafe_CartVect(nX, nY, nZ);

            double uX = 0;
            double uY = 0;
            double uZ = 1;
            Vector.MemorySafe_CartVect upVector = new Vector.MemorySafe_CartVect(uX, uY, uZ);

            //rotate the axis vectors for the future

            //ensure the vector passed into the function is a unit vector
            normalVector = Vector.UnitVector(normalVector);
            //get tilt:  cross product of normal vector and upVector
            //dot product equal to -1 is sign of antiparallelism
            double dot = Vector.DotProductMag(upVector, normalVector);
            if(dot+1 < .001)
            {
                calculatedTilt = 180;
                return calculatedTilt;
            }
            else
            {
                Vector.MemorySafe_CartVect tiltVector = Vector.CrossProduct(normalVector, upVector);
                double tiltVectorMagnitude = Vector.VectorMagnitude(tiltVector);
                calculatedTilt = Math.Round(Math.Asin(tiltVectorMagnitude) * 180 / Math.PI, 2);
                return calculatedTilt;
            }
        }

        static public double FindAzimuth(Vector.MemorySafe_CartVect normalVector)
        {
            double calculatedAzimuth = -999;
            //may need to also take into account other factors that, at this stage, seem to not be important
            //building Direction of Relative North
            //zone Direction of Relative North
            //GlobalGeometryRules coordinate system
            //I may need to know this in the future then rotate the axis vectors I am making below

            //x-axis [1 0 0] points east, y-axis [0 1 0] points north, z-axis[0 0 1] points up to the sky
            //alignment with y axis means north pointing, alignment with z-axis means it is pointing up to the sky (like a flat roof)

            Vector.MemorySafe_CartVect northVector = new Vector.MemorySafe_CartVect(0, 1, 0);

            Vector.MemorySafe_CartVect southVector = new Vector.MemorySafe_CartVect(0, -1, 0);

            Vector.MemorySafe_CartVect eastVector = new Vector.MemorySafe_CartVect(1, 0, 0);

            Vector.MemorySafe_CartVect westVector = new Vector.MemorySafe_CartVect(-1, 0, 0);

            Vector.MemorySafe_CartVect upVector = new Vector.MemorySafe_CartVect(0, 0, 1);

            //rotate the axis vectors for the future

            //ensure the vector passed into the function is a unit vector
            normalVector = Vector.UnitVector(normalVector);
            //get X-Y projection of the normal vector
            //normalVector.Z = 0;
            //get azimuth:  cross product of normal vector x-y projection and northVector
            //2-14-2014 we added last two statements to deal with normal
            //1st quadrant
            if ((normalVector.X == 0 && normalVector.Y == 1) || (normalVector.X == 1 && normalVector.Y == 0) || (normalVector.X > 0 && normalVector.Y > 0) || (normalVector.X > 0 && normalVector.Z != 0) || (normalVector.Y > 0 && normalVector.Z != 0))
            {
                //get azimuth:  cross product of normal vector x-y projection and northVector
                Vector.MemorySafe_CartVect azVector = Vector.CrossProduct(normalVector, northVector);
                double azVectorMagnitude = Vector.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2);
                return calculatedAzimuth;
            }
            //second quadrant
            else if ((normalVector.X < 0 && normalVector.Y > 0) )
            {
                Vector.MemorySafe_CartVect azVector = Vector.CrossProduct(normalVector, westVector);
                double azVectorMagnitude = Vector.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 270;
                return calculatedAzimuth;
            }
            //quadrant 3
            else if ((normalVector.X < 0 && normalVector.Y < 0) || (normalVector.X == -1 && normalVector.Y == 0) || (normalVector.X < 0 && normalVector.Z !=0))
            {
                Vector.MemorySafe_CartVect azVector = Vector.CrossProduct(normalVector, southVector);
                double azVectorMagnitude = Vector.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 180;
                return calculatedAzimuth;
            }
            //quadrant 4
            else if ((normalVector.X > 0 && normalVector.Y < 0) || (normalVector.X == 0 && normalVector.Y == -1) || (normalVector.Y < 0 && normalVector.Z != 0))
            {
                Vector.MemorySafe_CartVect azVector = Vector.CrossProduct(normalVector, eastVector);
                double azVectorMagnitude = Vector.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 90;
                return calculatedAzimuth;
            }
            //this will happen to vectors that point straight down or straight up because we are only interested in the X-Y projection and set the Z to zero anyways
            else if (normalVector.X == 0 && normalVector.Y == 0)
            {
                calculatedAzimuth = 0;
                return calculatedAzimuth;
            }

            //get the 

            return calculatedAzimuth;
        }

        static public SpaceBoundary MakeSpaceBoundary(SpaceBoundary sb, List<List<double>> points, int surfacecount)
        {
            sb.surfaceIdRef = "su-" + surfacecount.ToString();
            sb.PlanarGeometry = new PlanarGeometry();
            sb.PlanarGeometry.PolyLoop = new PolyLoop();
            sb.PlanarGeometry.PolyLoop = makePolyLoopsFromDbleList(sb.PlanarGeometry.PolyLoop, points);
            return sb;
        }
        //receives a polyloop and a list of points.  
        //the polyloop is returned filled with cartesian points
        //can only create a polyloop with 100 coordinates at the most
        static public PolyLoop makePolyLoopsFromDbleList(PolyLoop pg, List<List<double>> pointslist)
        {
            pg.Points = new CartesianPoint[pointslist.Count()];

            //culturally invariant by default
            int listcount = 0;
            foreach (List<double> ofpoints in pointslist)
            {
                //we assume that each set ofpoints has three coordinate
                //the first is an x coordinate, second is y, third is z
                //there will only be three doubles
                CartesianPoint pt = new CartesianPoint();
                pt.Coordinate = new string[3];
                CultureInfo ci = new CultureInfo(String.Empty);
                string xformat = string.Format(ci, "{0:0.000000}", ofpoints[0]);
                string yformat = string.Format(ci, "{0:0.000000}", ofpoints[1]);
                string zformat = string.Format(ci, "{0:0.000000}", ofpoints[2]);
                pt.Coordinate[0] = xformat;
                pt.Coordinate[1] = yformat;
                pt.Coordinate[2] = zformat;
                pg.Points[listcount] = pt;
                listcount++;

            }

            return pg;
        }

        //receives a polyloop and a list of vector memory safe coordinates  
        //the polyloop is returned filled with cartesian points
        //can only create a polyloop with 100 coordinates at the most
        static public PolyLoop MakePolyLoops(PolyLoop pg, List<Vector.MemorySafe_CartCoord> pointslist)
        {
            pg.Points = new CartesianPoint[pointslist.Count()];

            //culturally invariant by default
            int listcount = 0;
            foreach (Vector.MemorySafe_CartCoord point in pointslist)
            {
                //we assume that each set ofpoints has three coordinate
                //the first is an x coordinate, second is y, third is z
                //there will only be three doubles
                CartesianPoint pt = new CartesianPoint();
                pt.Coordinate = new string[3];
                CultureInfo ci = new CultureInfo(String.Empty);
                string xformat = string.Format(ci, "{0:0.000000}", point.X);
                string yformat = string.Format(ci, "{0:0.000000}", point.Y);
                string zformat = string.Format(ci, "{0:0.000000}", point.Z);
                pt.Coordinate[0] = xformat;
                pt.Coordinate[1] = yformat;
                pt.Coordinate[2] = zformat;
                pg.Points[listcount] = pt;
                listcount++;

            }

            return pg;
        }

        //this will be gone.  I just used it now to create a fake list of Coordinates in a plane
        static public List<List<double>> MakeFakeList(int numpoints)
        {
            List<List<double>> returnlist = new List<List<double>>();
            for (int j = 0; j < numpoints; j++)
            {
                List<double> onecoordinatelist = new List<double>();
                double placeholderdef = -999;
                for (int i = 0; i < 3; i++)
                {
                    onecoordinatelist.Add(placeholderdef);
                }
                returnlist.Add(onecoordinatelist);
            }

            return returnlist;
        }

        static public Surface SetUpSurfaceFromIDF(EPObj.MemorySafe_Surface epsurface, PlanarGeometry pg)
        {
            Surface retsurface = new Surface();
            retsurface.PlanarGeometry = pg;
            if (epsurface._sunExposureVar == "SunExposed")
            {
                retsurface.AdjacentSpaceId = new AdjacentSpaceId[1];
                if (epsurface.tilt > 45 && epsurface.tilt < 135)
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an exterior wall
                    retsurface.surfaceType = surfaceTypeEnum.ExteriorWall;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    AdjacentSpaceId adj = new AdjacentSpaceId();
                    adj.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = true;
                }
                //it can be a roof
                else if (epsurface.tilt >= 0 && epsurface.tilt <= 45)
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an exterior wall
                    retsurface.surfaceType = surfaceTypeEnum.Roof;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    AdjacentSpaceId adj = new AdjacentSpaceId();
                    adj.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = true;
                }
                //it can be an exposed floor
                else
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an exterior wall
                    retsurface.surfaceType = surfaceTypeEnum.UndergroundSlab;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    AdjacentSpaceId adj = new AdjacentSpaceId();
                    adj.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = true;
                }
            }
            else if (epsurface._sunExposureVar == "NoSun" && epsurface._outsideBoundaryCondition == "Ground")
            {
                if (epsurface.tilt > 45 && epsurface.tilt < 135)
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an underground wall
                    retsurface.surfaceType = surfaceTypeEnum.UndergroundWall;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    AdjacentSpaceId adj1 = new AdjacentSpaceId();
                    adj1.spaceIdRef = epsurface.zoneName;
                    AdjacentSpaceId adj2 = new AdjacentSpaceId();
                    adj2.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj1;
                    retsurface.AdjacentSpaceId[1] = adj2;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = false;
                }
                else if (epsurface.tilt >= 0 && epsurface.tilt <= 45)
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an underground ceiling
                    retsurface.surfaceType = surfaceTypeEnum.UndergroundCeiling;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    AdjacentSpaceId adj1 = new AdjacentSpaceId();
                    adj1.spaceIdRef = epsurface.zoneName;
                    AdjacentSpaceId adj2 = new AdjacentSpaceId();
                    adj2.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj1;
                    retsurface.AdjacentSpaceId[1] = adj2;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = false;
                }
                else
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an underground slab or slab on grade

                    retsurface.surfaceType = surfaceTypeEnum.UndergroundSlab;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    AdjacentSpaceId adj1 = new AdjacentSpaceId();
                    adj1.spaceIdRef = epsurface.zoneName;
                    AdjacentSpaceId adj2 = new AdjacentSpaceId();
                    adj2.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj1;
                    retsurface.AdjacentSpaceId[1] = adj2;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = false;
                }
            }
            else
            {
                retsurface.AdjacentSpaceId = new AdjacentSpaceId[2];
                //some new code associated with finding the order of the two spaces
                if (epsurface.tilt > 45 && epsurface.tilt < 135)
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an underground wall
                    retsurface.surfaceType = surfaceTypeEnum.UndergroundWall;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    //this is wrong
                    AdjacentSpaceId adj1 = new AdjacentSpaceId();
                    adj1.spaceIdRef = epsurface.zoneName;
                    AdjacentSpaceId adj2 = new AdjacentSpaceId();
                    adj2.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj1;
                    retsurface.AdjacentSpaceId[1] = adj2;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = false;
                }
                else if (epsurface.tilt >= 0 && epsurface.tilt <= 45)
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an underground ceiling
                    retsurface.surfaceType = surfaceTypeEnum.UndergroundCeiling;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    //this is wrong
                    AdjacentSpaceId adj1 = new AdjacentSpaceId();
                    adj1.spaceIdRef = epsurface.zoneName;
                    AdjacentSpaceId adj2 = new AdjacentSpaceId();
                    adj2.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj1;
                    retsurface.AdjacentSpaceId[1] = adj2;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = false;
                }
                else
                {
                    Dictionary<string, double> WH = new Dictionary<string, double>();
                    //it can be considered an underground slab or slab on grade

                    retsurface.surfaceType = surfaceTypeEnum.UndergroundSlab;
                    retsurface.constructionIdRef = "something";
                    retsurface.Name = epsurface.name;
                    //this is wrong
                    AdjacentSpaceId adj1 = new AdjacentSpaceId();
                    adj1.spaceIdRef = epsurface.zoneName;
                    AdjacentSpaceId adj2 = new AdjacentSpaceId();
                    adj2.spaceIdRef = epsurface.zoneName;
                    retsurface.AdjacentSpaceId[0] = adj1;
                    retsurface.AdjacentSpaceId[1] = adj2;

                    RectangularGeometry rg = new RectangularGeometry();
                    rg.Azimuth = gb.FormatDoubleToString(epsurface.azimuth);
                    //find lower left hand corner of exterior wall
                    //for now, we will just arbitrarily choose a point
                    rg.CartesianPoint = pg.PolyLoop.Points[0];
                    rg.Tilt = gb.FormatDoubleToString(epsurface.tilt);
                    //get width and height
                    WH = GetWidthandHeight(epsurface, retsurface.surfaceType);
                    rg.Width = gb.FormatDoubleToString(WH["width"]);
                    rg.Height = gb.FormatDoubleToString(WH["height"]);
                    retsurface.RectangularGeometry = rg;

                    retsurface.PlanarGeometry = pg;
                    retsurface.exposedToSunField = false;
                }
            }
            return retsurface;
        }


        static public Dictionary<string, double> GetWidthandHeight(EPObj.MemorySafe_Surface epsurface, surfaceTypeEnum surftype)
        {
            Dictionary<string, double> WH = new Dictionary<string, double>();
            List<EPObj.MemorySafe_CartCoord> coords = epsurface.SurfaceCoords;
            //we use a very simple algorithm where we find the area and the max height
            //we divide the area by the max height, and this is our width.
            //this helps us cover the case of non-vertical surfaces, and polygons that
            //are not rectangles

            double width = 0;
            double height = 0;
            for (int i = 0; i < (coords.Count() - 1); i++)
            {

                double dx = Math.Abs(coords[i].X - coords[i + 1].X);
                double dy = Math.Abs(coords[i].Y - coords[i + 1].Y);
                double dz = Math.Abs(coords[i].Z - coords[i + 1].Z);
                //get the height
                if (surftype == surfaceTypeEnum.ExteriorWall || surftype == surfaceTypeEnum.InteriorWall || surftype == surfaceTypeEnum.UndergroundWall)
                {
                    if (dz == 0) continue;
                    else
                    {
                        if (dx == 0 && dy == 0)
                        {
                            if (dz > height) { height = dz; }
                            else continue;
                        }
                        else
                        {
                            //can you prove this somehow?
                            double dist = Math.Sqrt(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) + Math.Pow(dz, 2));
                            if (dist > height) { height = dist; }
                            else continue;
                        }
                    }
                }
                else
                {
                    if (dz == 0)
                    {
                        double dist = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                        if (dist > height) { height = dist; }
                        else continue;
                    }
                    else //it is tilted
                    {
                        double dist = Math.Sqrt(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) + Math.Pow(dz, 2));
                        if (dist > height) { height = dist; }
                        else continue;
                    }
                }
                //this simplifies the definition of width and area greatly
                double area = EPObj.GetAreaofSurface(epsurface);
                if (height != 0)
                {
                    width = area / height;
                }
            }
            WH.Add("width", width);
            WH.Add("height", height);
            return WH;
        }

        static List<EPObj.MemorySafe_CartCoord> ConvertCoordToDouble(CartesianPoint[] points)
        {
            List<EPObj.MemorySafe_CartCoord> coords = new List<EPObj.MemorySafe_CartCoord>();
            foreach (CartesianPoint pt in points)
            {
                double x = Convert.ToDouble(pt.Coordinate[0]);
                double y = Convert.ToDouble(pt.Coordinate[1]);
                double z = Convert.ToDouble(pt.Coordinate[2]);
                EPObj.MemorySafe_CartCoord coord = new EPObj.MemorySafe_CartCoord(x, y, z);
                coords.Add(coord);
            }

            return coords;
        }

        public static PolyLoop[] makePolyLoopArray(int size)
        {
            PolyLoop[] retarr = new PolyLoop[size];
            for(int i = 0; i<size; i++)
            {
                PolyLoop pl = new PolyLoop();
                retarr[i] = pl;
            }
            return retarr;
        }

        public static Space[] makeSpaceArray(int size)
        {
            return new Space[size];
        }

        public static Surface[] makeSurfaceArray(int size)
        {
            return new Surface[size];
        }

    }
}
