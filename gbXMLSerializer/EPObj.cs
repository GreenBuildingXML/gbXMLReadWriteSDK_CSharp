using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gbXMLSerializer
{
    public class EPObj
    {
        public EPObj()
        {
        }

        public class CartCoord
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }

        public class MemorySafe_CartCoord
        {
            private readonly double _X, _Y, _Z;

            public MemorySafe_CartCoord(double X, double Y, double Z)
            {
                _X = X;
                _Y = Y;
                _Z = Z;
            }

            public double X { get { return _X; } }
            public double Y { get { return _Y; } }
            public double Z { get { return _Z; } }
        }

        public class CartVect
        {
            public CartVect()
            {

            }

            private double _X, _Y, _Z;
            public CartVect(double X, double Y, double Z)
            {
                _X = X;
                _Y = Y;
                _Z = Z;
            }
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

        }

        public class MemorySafe_CartVect
        {
            private readonly double _X, _Y, _Z;

            public MemorySafe_CartVect(double X, double Y, double Z)
            {
                _X = X;
                _Y = Y;
                _Z = Z;
            }

            public double X { get { return _X; } }
            public double Y { get { return _Y; } }
            public double Z { get { return _Z; } }

        }

        public class Spaces
        {
            public string name;
            public int multiplier;
            public List<Surface> spaceSurfaces;

        }

        public class EPSurface
        {
            public string name;
            public SurfaceTypes surfaceType;
            public string constructionName;
            public OutsideBoundary outsideBoundary;
            public enum SurfaceTypes
            {
                Wall,
                Floor,
                Ceiling,
                Roof,
                Blank
            }
            public enum OutsideBoundary
            {
                Surface,
                Ground,
                Outdoors,
                Zone,
                OtherSideCoefficients,
                OtherSideConditionsModel,
                Blank

            }
            public string zoneName;
            public string outsideBoundaryCondition;
            public string sunExposureVar;
            public string windExposureVar;
            public double viewFactor;
            public int numVertices;
            public List<CartCoord> SurfaceCoords;
            public double tilt;
            public double azimuth;

            public void Clear()
            {
                name = "";
                surfaceType = SurfaceTypes.Blank;
                constructionName = "";
                outsideBoundary = OutsideBoundary.Blank;
                zoneName = "";
                sunExposureVar = "";
                windExposureVar = "";
                numVertices = 0;
                SurfaceCoords.Clear();

            }

        }

        public class Surface
        {
            public string name;
            public int multiplier;
            public SurfaceTypes surfaceType;
            public string constructionName;
            public OutsideBoundary outsideBoundary;
            public string zoneName;
            public string outsideBoundaryCondition;
            public string sunExposureVar;
            public string windExposureVar;
            public double viewFactor;
            public int numVertices;
            public List<CartCoord> SurfaceCoords;
            public double tilt;
            public double azimuth;

            public void Clear()
            {
                name = "";
                surfaceType = SurfaceTypes.Blank;
                constructionName = "";
                outsideBoundary = OutsideBoundary.Blank;
                zoneName = "";
                sunExposureVar = "";
                windExposureVar = "";
                numVertices = 0;
                SurfaceCoords.Clear();

            }

        }

        public class MemorySafe_Spaces
        {
            private readonly string _name;
            private readonly int _multiplier;
            private readonly List<MemorySafe_Surface> _spaceSurfaces;

            public MemorySafe_Spaces(string name, int multiplier, List<MemorySafe_Surface> spaceSurfaces)
            {
                _name = name;
                _multiplier = multiplier;
                _spaceSurfaces = spaceSurfaces;
            }

            public string name { get { return _name; } }
            public int multiplier { get { return _multiplier; } }
            public List<MemorySafe_Surface> spaceSurfaces { get { return _spaceSurfaces; } }

        }

        public enum OutsideBoundary
        {
            Surface,
            Ground,
            Outdoors,
            Zone,
            OtherSideCoefficients,
            OtherSideConditionsModel,
            Blank

        }
        public enum SurfaceTypes
        {
            Wall,
            Floor,
            Ceiling,
            Roof,
            Blank
        }

        public class MemorySafe_Surface
        {

            public string _name;
            public int _multiplier;
            public SurfaceTypes _surfaceType;
            public string _constructioName;
            public OutsideBoundary _outsideBoundary;
            public string _zoneName;
            public string _outsideBoundaryCondition;
            public string _sunExposureVar;
            public string _windExposureVar;
            public double _viewFactor;
            public int _numVertices;
            public List<MemorySafe_CartCoord> _SurfaceCoords;
            public double _tilt;
            public double _azimuth;

            public MemorySafe_Surface()
            {

            }

            public MemorySafe_Surface(string name, int multiplier, SurfaceTypes surfaceType, string constName, OutsideBoundary ob,
                string zoneName, string outsideBC, string sunExp, string windExp, double vF, int numVert, List<MemorySafe_CartCoord> sC,
                double tilt, double az)
            {
                _name = name;
                _multiplier = multiplier;
                _surfaceType = surfaceType;
                _constructioName = constName;
                _outsideBoundary = ob;
                _zoneName = zoneName;
                _outsideBoundaryCondition = outsideBC;
                _sunExposureVar = sunExp;
                _windExposureVar = windExp;
                _viewFactor = vF;
                _numVertices = numVert;
                _SurfaceCoords = sC;
                _tilt = tilt;
                _azimuth = az;

            }

            public string name { get { return _name; } }
            public int multiplier { get { return _multiplier; } }
            public SurfaceTypes surfaceType { get { return _surfaceType; } }
            public string constructionName { get { return _constructioName; } }
            public OutsideBoundary outsideBoundary { get { return _outsideBoundary; } }
            public string zoneName { get { return _zoneName; } }
            public string outsideBoundaryCondition { get { return _outsideBoundaryCondition; } }
            public string sunExposureVar { get { return _sunExposureVar; } }
            public string windExposureVar { get { return _windExposureVar; } }
            public double viewFactor { get { return _viewFactor; } }
            public int numVertices { get { return _numVertices; } }
            public List<MemorySafe_CartCoord> SurfaceCoords { get { return _SurfaceCoords; } }
            public double tilt { get { return _tilt; } }
            public double azimuth { get { return _azimuth; } }

        }

        public static MemorySafe_Surface convert2MemorySafeSurface(Surface surface)
        {
            List<MemorySafe_CartCoord> surfaceCoords = new List<MemorySafe_CartCoord>();
            foreach (CartCoord coord in surface.SurfaceCoords)
            {
                MemorySafe_CartCoord surfaceCoord = new MemorySafe_CartCoord(coord.X, coord.Y, coord.Z);
                surfaceCoords.Add(surfaceCoord);
            }

            MemorySafe_Surface memSurface = new MemorySafe_Surface(surface.name, surface.multiplier, surface.surfaceType, surface.constructionName,
                surface.outsideBoundary, surface.zoneName, surface.outsideBoundaryCondition, surface.sunExposureVar, surface.windExposureVar,
                surface.viewFactor, surface.numVertices, surfaceCoords, surface.tilt, surface.azimuth);

            return memSurface;

        }

        public static CartVect GetRHR(List<CartCoord> plCoords)
        {
            CartVect plRHRVect = new CartVect();
            //this list will store all of the rhr values returned by any arbitrary polyloop
            List<CartVect> RHRs = new List<CartVect>();

            int coordCount = plCoords.Count;
            for (int i = 0; i < coordCount - 2; i++)
            {
                CartVect v1 = CreateVector(plCoords[i], plCoords[i + 1]);
                CartVect v2 = CreateVector(plCoords[i + 1], plCoords[i + 2]);
                CartVect uv = UnitVector(CrossProduct(v1, v2));
                RHRs.Add(uv);
            }
            int RHRVectorCount = RHRs.Count;
            List<CartVect> distinctRHRs = new List<CartVect>();
            int parallelCount = 0;
            int antiParallelCount = 0;
            //the Distinct().ToList() routine did not work because, we believe, the item in the list is not recognized by Distinct()
            //distinctRHRs = RHRs.Distinct().ToList();
            //so we took the following approach to try and find unique vectors and store them
            distinctRHRs.Add(RHRs[0]);
            List<int> uniqueIndices = new List<int>();
            for (int j = 1; j < RHRVectorCount; j++)
            {

                if (RHRs[j].X == distinctRHRs[0].X * -1 && RHRs[j].Y == distinctRHRs[0].Y * -1 && RHRs[j].Z == distinctRHRs[0].Z * -1)
                {
                    //means that the vectors are not facing in the same direction
                    antiParallelCount++;
                }
                else
                {
                    parallelCount++;
                }

            }

            if (antiParallelCount > parallelCount)
            {
                CartVect antiParallel = new CartVect { };
                antiParallel.X = distinctRHRs[0].X * -1;
                antiParallel.Y = distinctRHRs[0].Y * -1;
                antiParallel.Z = distinctRHRs[0].Z * -1;

                return antiParallel;
            }
            else
            {
                return distinctRHRs[0];
            }
        }

        public static Double VectorMagnitude(MemorySafe_CartVect vector)
        {
            double magnitude = 0.0;

            magnitude = Math.Sqrt(Math.Pow((vector.X), 2) + Math.Pow((vector.Y), 2) + Math.Pow((vector.Z), 2));
            return magnitude;
        }

        public static CartVect CreateVector(CartCoord cd1, CartCoord cd2)
        {
            CartVect vector = new CartVect();
            vector.X = cd2.X - cd1.X;
            vector.Y = cd2.Y - cd1.Y;
            vector.Z = cd2.Z - cd1.Z;
            return vector;
        }

        public static CartVect UnitVector(CartVect vector)
        {
            CartVect UV = new CartVect();
            double magnitude = VectorMagnitude(vector);

            UV.X = vector.X / magnitude;
            UV.Y = vector.Y / magnitude;
            UV.Z = vector.Z / magnitude;
            return UV;
        }

        public static MemorySafe_CartVect UnitVector(MemorySafe_CartVect vector)
        {
            double magnitude = VectorMagnitude(vector);
            double X = vector.X / magnitude;
            double Y = vector.Y / magnitude;
            double Z = vector.Z / magnitude;
            MemorySafe_CartVect UV = new MemorySafe_CartVect(X, Y, Z);
            return UV;
        }

        public static CartVect CrossProductNVRetMSNV(MemorySafe_CartVect vector1, CartVect vector2)
        {
            double xProdX = vector2.Z * vector1.Y - vector1.Z * vector2.Y;
            double xProdY = -1 * (vector2.Z * vector1.X - vector1.Z * vector2.X);
            double xProdZ = vector2.Y * vector1.X - vector1.Y * vector2.X;
            CartVect xProd = new CartVect(xProdX, xProdY, xProdZ);
            xProd.X = xProdX;
            xProd.Y = xProdY;
            xProd.Z = xProdZ;
            return xProd;
        }

        public static Double VectorMagnitude(CartVect vector)
        {
            double magnitude = 0.0;

            magnitude = Math.Sqrt(Math.Pow((vector.X), 2) + Math.Pow((vector.Y), 2) + Math.Pow((vector.Z), 2));
            return magnitude;
        }


        public static CartVect CrossProduct(CartVect vector1, CartVect vector2)
        {
            CartVect xProd = new CartVect();

            xProd.X = vector2.Z * vector1.Y - vector1.Z * vector2.Y;
            xProd.Y = -1 * (vector2.Z * vector1.X - vector1.Z * vector2.X);
            xProd.Z = vector2.Y * vector1.X - vector1.Y * vector2.X;
            return xProd;
        }

        public static MemorySafe_CartVect CrossProduct(MemorySafe_CartVect vector1, MemorySafe_CartVect vector2)
        {
            double xProdX = vector2.Z * vector1.Y - vector1.Z * vector2.Y;
            double xProdY = -1 * (vector2.Z * vector1.X - vector1.Z * vector2.X);
            double xProdZ = vector2.Y * vector1.X - vector1.Y * vector2.X;
            MemorySafe_CartVect xProd = new MemorySafe_CartVect(xProdX, xProdY, xProdZ);
            return xProd;
        }


        public static MemorySafe_CartVect convertToMemorySafeVector(CartVect vector)
        {
            MemorySafe_CartVect memVect = new MemorySafe_CartVect(vector.X, vector.Y, vector.Z);
            return memVect;
        }

        public static MemorySafe_CartCoord convertToMemorySafeCoord(CartCoord coord)
        {
            MemorySafe_CartCoord memCoord = new MemorySafe_CartCoord(coord.X, coord.Y, coord.Z);
            return memCoord;
        }


        public static double GetAreaofSurface(MemorySafe_Surface surface)
        {
            //Used to figure out how best to calculate the area from a given surfacce. 
            //Get the coordinates that define the surface
            //get the area based on the coordinates

            //Get the RHRVector (the actual direction is not important
            MemorySafe_CartVect RHRVector = GetRHR(surface.SurfaceCoords);

            //now that I have this, I can move on

            //there are two basic cases for calculating the area that we cover here, 
            //one where we get the area using greens theorem when the surface is parallel to one of the axes of the project global reference frame
            //and the second where the surface is not parallel to one of the axes of the global reference frame

            //Surface normal Parallel to global reference frame X Axis
            if (Math.Abs(RHRVector.X) == 1 && RHRVector.Y == 0 && RHRVector.Z == 0)
            {
                List<MemorySafe_CartCoord> coordList = new List<MemorySafe_CartCoord>();
                for (int i = 0; i < surface.SurfaceCoords.Count; i++)
                {
                    //only take the Y and Z coordinates and throw out the X because we can assume that they are all the same
                    double X = 0;
                    double Y = surface.SurfaceCoords[i].Y;
                    double Z = surface.SurfaceCoords[i].Z;
                    MemorySafe_CartCoord coord = new MemorySafe_CartCoord(X, Y, Z);
                    coordList.Add(coord);

                }
                double area = GetAreaFrom2DPolyLoop(coordList);
                return area;


            }
            //Surface normal Parallel to global reference frame y Axis
            else if (RHRVector.X == 0 && Math.Abs(RHRVector.Y) == 1 && RHRVector.Z == 0)
            {
                List<MemorySafe_CartCoord> coordList = new List<MemorySafe_CartCoord>();
                for (int i = 0; i < surface.SurfaceCoords.Count; i++)
                {
                    //only take the X and Z coordinates and throw out the Y because we can assume that they are all the same
                    double X = surface.SurfaceCoords[i].X;
                    double Y = 0;
                    double Z = surface.SurfaceCoords[i].Z;
                    MemorySafe_CartCoord coord = new MemorySafe_CartCoord(X, Y, Z);
                    coordList.Add(coord);

                }
                double area = GetAreaFrom2DPolyLoop(coordList);
                return area;
            }
            else if (RHRVector.X == 0 && RHRVector.Y == 0 && Math.Abs(RHRVector.Z) == 1)
            {
                List<MemorySafe_CartCoord> coordList = new List<MemorySafe_CartCoord>();
                for (int i = 0; i < surface.SurfaceCoords.Count; i++)
                {
                    //only take the X and Y coordinates and throw out the Z because we can assume that they are all the same
                    double X = surface.SurfaceCoords[i].X;
                    double Y = surface.SurfaceCoords[i].Y;
                    double Z = 0;
                    MemorySafe_CartCoord coord = new MemorySafe_CartCoord(X, Y, Z);
                    coordList.Add(coord);
                }
                double area = GetAreaFrom2DPolyLoop(coordList);
                return area;
            }

            //the surface is not aligned with one of the reference frame axes, which requires a bit more work to determine the right answer.
            else
            {

                //New Z Axis for this plane is the normal vector already calculated, does not need to be created
                //Get New Y Axis which is the surface Normal Vector cross the original global reference X unit vector (all unit vectors please
                double X = 1;
                double Y = 0;
                double Z = 0;
                MemorySafe_CartVect globalReferenceX = new MemorySafe_CartVect(X, Y, Z);

                MemorySafe_CartVect localY = CrossProduct(RHRVector, globalReferenceX);
                localY = UnitVector(localY);

                //new X axis is the localY cross the surface normal vector
                MemorySafe_CartVect localX = CrossProduct(localY, RHRVector);
                localX = UnitVector(localX);

                //convert the polyloop coordinates to a local 2-D reference frame
                //using a trick employed by video game programmers found here http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane
                List<MemorySafe_CartCoord> translatedCoordinates = new List<MemorySafe_CartCoord>();
                //put the origin in place in these translated coordinates since our loop skips over this first arbitrary point
                double originX = 0;
                double originY = 0;
                double originZ = 0;
                MemorySafe_CartCoord newOrigin = new MemorySafe_CartCoord(originX, originY, originZ);
                translatedCoordinates.Add(newOrigin);
                for (int j = 1; j < surface.SurfaceCoords.Count; j++)
                {
                    //randomly assigns the first polyLoop coordinate as the origin
                    MemorySafe_CartCoord origin = surface.SurfaceCoords[0];
                    //captures the components of a vector drawn from the new origin to the 
                    double xDistance = surface.SurfaceCoords[j].X - origin.X;
                    double yDist = surface.SurfaceCoords[j].Y - origin.Y;
                    double zDist = surface.SurfaceCoords[j].Z - origin.Z;
                    MemorySafe_CartVect distance = new MemorySafe_CartVect(xDistance, yDist, zDist);
                    double translPtX = distance.X * localX.X + distance.Y * localX.Y + distance.Z * localX.Z;
                    double translPtY = distance.X * localY.X + distance.Y * localY.Y + distance.Z * localY.Z;
                    double translPtZ = 0;
                    MemorySafe_CartCoord translatedPt = new MemorySafe_CartCoord(translPtX, translPtY, translPtZ);
                    translatedCoordinates.Add(translatedPt);

                }
                double area = GetAreaFrom2DPolyLoop(translatedCoordinates);
                return area;
            }

        }

        public static double GetAreaFrom2DPolyLoop(List<MemorySafe_CartCoord> coordList)
        {
            int count = coordList.Count;
            double areaprod = 0;
            bool XisZero = true;
            bool YisZero = true;
            bool ZisZero = true;
            //the following calculates the area of any irregular polygon
            foreach (MemorySafe_CartCoord coord in coordList)
            {
                if (coord.X != 0) XisZero = false;
                if (coord.Y != 0) YisZero = false;
                if (coord.Z != 0) ZisZero = false;
            }

            //since we can only get the area of a 2-D projection, if all three of the coordinates are not zero, then we would have to
            //return an error since this is not possible
            if (!XisZero && !YisZero && !ZisZero) return -999;

            //the rest uses greens theorem
            if (XisZero)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < count - 1)
                    {
                        areaprod += (coordList[i].Y * coordList[i + 1].Z - coordList[i].Z * coordList[i + 1].Y);
                    }
                    else if (i == count - 1)
                    {
                        areaprod += (coordList[i].Y * coordList[0].Z - coordList[i].Z * coordList[0].Y);
                    }
                }
                areaprod /= 2;
            }
            else if (YisZero)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < count - 1)
                    {
                        areaprod += (coordList[i].X * coordList[i + 1].Z - coordList[i].Z * coordList[i + 1].X);
                    }
                    else if (i == count - 1)
                    {
                        areaprod += (coordList[i].X * coordList[0].Z - coordList[i].Z * coordList[0].X);
                    }
                }
                areaprod /= 2;

            }
            else if (ZisZero)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i < count - 1)
                    {
                        areaprod += (coordList[i].X * coordList[i + 1].Y - coordList[i].Y * coordList[i + 1].X);
                    }
                    else if (i == count - 1)
                    {
                        areaprod += (coordList[i].X * coordList[0].Y - coordList[i].Y * coordList[0].X);
                    }
                }
                areaprod /= 2;
            }
            return Math.Abs(areaprod);
        }

        public static MemorySafe_CartVect GetRHR(List<MemorySafe_CartCoord> plCoords)
        {
            CartVect plRHRVect = new CartVect();
            //this list will store all of the rhr values returned by any arbitrary polyloop
            List<MemorySafe_CartVect> RHRs = new List<MemorySafe_CartVect>();

            int coordCount = plCoords.Count;
            for (int i = 0; i < coordCount - 2; i++)
            {
                MemorySafe_CartVect v1 = CreateMemorySafe_Vector(plCoords[i], plCoords[i + 1]);
                MemorySafe_CartVect v2 = CreateMemorySafe_Vector(plCoords[i + 1], plCoords[i + 2]);
                MemorySafe_CartVect uv = UnitVector(CrossProduct(v1, v2));
                RHRs.Add(uv);
            }
            int RHRVectorCount = RHRs.Count;
            List<MemorySafe_CartVect> distinctRHRs = new List<MemorySafe_CartVect>();
            int parallelCount = 0;
            int antiParallelCount = 0;
            //the Distinct().ToList() routine did not work because, we believe, the item in the list is not recognized by Distinct()
            //distinctRHRs = RHRs.Distinct().ToList();
            //so we took the following approach to try and find unique vectors and store them
            distinctRHRs.Add(RHRs[0]);
            List<int> uniqueIndices = new List<int>();
            for (int j = 1; j < RHRVectorCount; j++)
            {

                if (RHRs[j].X == distinctRHRs[0].X * -1 && RHRs[j].Y == distinctRHRs[0].Y * -1 && RHRs[j].Z == distinctRHRs[0].Z * -1)
                {
                    //means that the vectors are not facing in the same direction
                    antiParallelCount++;
                }
                else
                {
                    parallelCount++;
                }

            }

            if (antiParallelCount > parallelCount)
            {
                double X = distinctRHRs[0].X * -1;
                double Y = distinctRHRs[0].Y * -1;
                double Z = distinctRHRs[0].Z * -1;
                MemorySafe_CartVect antiParallel = new MemorySafe_CartVect(X, Y, Z);
                return antiParallel;
            }
            else
            {
                return distinctRHRs[0];
            }
        }


        public static MemorySafe_CartVect CreateMemorySafe_Vector(MemorySafe_CartCoord cd1, MemorySafe_CartCoord cd2)
        {
            double X = cd2.X - cd1.X;
            double Y = cd2.Y - cd1.Y;
            double Z = cd2.Z - cd1.Z;
            MemorySafe_CartVect vector = new MemorySafe_CartVect(X, Y, Z);
            return vector;
        }

        public static MemorySafe_CartVect CreateMemorySafe_Vector(CartCoord cd1, CartCoord cd2)
        {
            double X = cd2.X - cd1.X;
            double Y = cd2.Y - cd1.Y;
            double Z = cd2.Z - cd1.Z;
            MemorySafe_CartVect vector = new MemorySafe_CartVect(X, Y, Z);
            return vector;
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

        static public double FindAzimuth(EPObj.MemorySafe_CartVect normalVector)
        {
            double calculatedAzimuth = -999;
            //may need to also take into account other factors that, at this stage, seem to not be important
            //building Direction of Relative North
            //zone Direction of Relative North
            //GlobalGeometryRules coordinate system
            //I may need to know this in the future then rotate the axis vectors I am making below

            //x-axis [1 0 0] points east, y-axis [0 1 0] points north, z-axis[0 0 1] points up to the sky
            //alignment with y axis means north pointing, alignment with z-axis means it is pointing up to the sky (like a flat roof)

            EPObj.MemorySafe_CartVect northVector = new EPObj.MemorySafe_CartVect(0,1,0);

            EPObj.MemorySafe_CartVect southVector = new EPObj.MemorySafe_CartVect(0,-1,0);

            EPObj.MemorySafe_CartVect eastVector = new EPObj.MemorySafe_CartVect(1,0,0);

            EPObj.MemorySafe_CartVect westVector = new EPObj.MemorySafe_CartVect(-1,0,0);

            EPObj.MemorySafe_CartVect upVector = new EPObj.MemorySafe_CartVect(0,0,1);

            //rotate the axis vectors for the future

            //ensure the vector passed into the function is a unit vector
            normalVector = EPObj.UnitVector(normalVector);
            //get X-Y projection of the normal vector
            //normalVector.Z = 0;
            //get azimuth:  cross product of normal vector x-y projection and northVector
            //1st quadrant
            if ((normalVector.X == 0 && normalVector.Y == 1) || (normalVector.X == 1 && normalVector.Y == 0) || (normalVector.X > 0 && normalVector.Y > 0))
            {
                //get azimuth:  cross product of normal vector x-y projection and northVector
                EPObj.MemorySafe_CartVect azVector = EPObj.CrossProduct(normalVector, northVector);
                double azVectorMagnitude = EPObj.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2);
                return calculatedAzimuth;
            }
            //second quadrant
            else if (normalVector.X < 0 && normalVector.Y > 0)
            {
                EPObj.MemorySafe_CartVect azVector = EPObj.CrossProduct(normalVector, westVector);
                double azVectorMagnitude = EPObj.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 270;
                return calculatedAzimuth;
            }
            //quadrant 3
            else if ((normalVector.X < 0 && normalVector.Y < 0) || (normalVector.X == -1 && normalVector.Y == 0))
            {
                EPObj.MemorySafe_CartVect azVector = EPObj.CrossProduct(normalVector, southVector);
                double azVectorMagnitude = EPObj.VectorMagnitude(azVector);

                //modification for when the vector is in different quadrants
                calculatedAzimuth = Math.Round(Math.Asin(azVectorMagnitude) * 180 / Math.PI, 2) + 180;
                return calculatedAzimuth;
            }
            //quadrant 4
            else if ((normalVector.X > 0 && normalVector.Y < 0) || (normalVector.X == 0 && normalVector.Y == -1))
            {
                EPObj.MemorySafe_CartVect azVector = EPObj.CrossProduct(normalVector, eastVector);
                double azVectorMagnitude = EPObj.VectorMagnitude(azVector);

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

    }
}
