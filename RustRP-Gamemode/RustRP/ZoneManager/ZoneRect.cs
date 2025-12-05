using UnityEngine;

namespace CoreRP.ZoneManager
{
    internal struct ZoneRect
    {
        public float Height { get; set; }
        public float Rotation { get; set; }
        public Vector3[] Points { get; set; }
        internal Vector3[] Bounds { get; }
        internal Vector3 Center { get; }
        internal Vector3 Size { get; }
        internal ZoneRect(Vector3[] points, float height = 10, float rotation = 0)
        {
            var min = ZoneHelper.MinPoint(points);
            var max = ZoneHelper.MaxPoint(points);

            var XmaxYmax = new Vector3(max.x, points[0].y, max.z);
            var XmaxYmin = new Vector3(max.x, points[1].y, min.z);
            var XminYmin = new Vector3(min.x, points[2].y, min.z);
            var XminYmax = new Vector3(min.x, points[3].y, max.z);

            var minHeight = ZoneHelper.MinPoint(XmaxYmax, XmaxYmin, XminYmin, XminYmax);
            var maxHeight = ZoneHelper.MaxPoint(XmaxYmax, XmaxYmin, XminYmin, XminYmax);

            this.Height = height;
            this.Rotation = rotation;
            this.Points = points;
            this.Bounds = new Vector3[8]
            {
                new Vector3(XmaxYmax.x, minHeight.y, XmaxYmax.z),
                new Vector3(XmaxYmin.x, minHeight.y, XmaxYmin.z),
                new Vector3(XminYmin.x, minHeight.y, XminYmin.z),
                new Vector3(XminYmax.x, minHeight.y, XminYmax.z),
                new Vector3(XmaxYmax.x, maxHeight.y + height, XmaxYmax.z),
                new Vector3(XmaxYmin.x, maxHeight.y + height, XmaxYmin.z),
                new Vector3(XminYmin.x, maxHeight.y + height, XminYmin.z),
                new Vector3(XminYmax.x, maxHeight.y + height, XminYmax.z),
            };
            this.Center = ZoneHelper.CenteredPoint(Bounds);
            this.Size = ZoneHelper.MaxPoint(Bounds) - ZoneHelper.MinPoint(Bounds);
        }
    }
}