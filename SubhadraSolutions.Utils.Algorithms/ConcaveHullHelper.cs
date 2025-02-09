//using SubhadraSolutions.Utils.Drawing;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SubhadraSolutions.Utils.Algorithms
//{
//   public class ConcaveHull
//    {
//        struct LineSegment
//        {
//            public PointF A { get; private set; }
//            public PointF B { get; private set; }

//            public LineSegment(PointF a, PointF b)
//            {
//                this.A = a;
//                this.B = b;
//            }
//        }

//        private double threshold;

//        public Dictionary<LineSegment, int> segments = new Dictionary<LineSegment, int>();
//        public Dictionary<int, Edge> edges = new Dictionary<int, Edge>();
//        public Dictionary<int, Triangle> triangles = new Dictionary<int, Triangle>();
//        public TreeMap<int, Edge> lengths = new TreeMap<int, Edge>();

//        public Dictionary<int, Edge> shortLengths = new Dictionary<int, Edge>();

//        public Dictionary<PointF, int> coordinates = new Dictionary<PointF, int>();
//        public Dictionary<int, Vertex> vertices = new Dictionary<int, Vertex>();

//        /**
//         * Create a new concave hull construction for the input {@link Geometry}.
//         *
//         * @param geometry
//         * @param threshold
//         */
//        public ConcaveHull(Geometry geometry, double threshold)
//        {
//            this.geometries = transformIntoPointGeometryCollection(geometry);
//            this.threshold = threshold;
//            this.geomFactory = geometry.getFactory();
//        }

//        /**
//         * Create a new concave hull construction for the input
//         * {@link GeometryCollection}.
//         *
//         * @param geometries
//         * @param threshold
//         */
//        public ConcaveHull(GeometryCollection geometries, double threshold)
//        {
//            this.geometries = transformIntoPointGeometryCollection(geometries);
//            this.threshold = threshold;
//            this.geomFactory = geometries.getFactory();
//        }

//        /**
//         * Create the concave hull.
//         *
//         * @return the concave hull
//         */
//        @SuppressWarnings("unchecked")
//    private Geometry concaveHull()
//        {
//            // triangulation: create a DelaunayTriangulationBuilder object
//            ConformingDelaunayTriangulationBuilder cdtb = new ConformingDelaunayTriangulationBuilder();

//            // add geometry collection
//            cdtb.setSites(this.geometries);

//            QuadEdgeSubdivision qes = cdtb.getSubdivision();

//            Collection<QuadEdge> quadEdges = qes.getEdges();
//            List<QuadEdgeTriangle> qeTriangles = QuadEdgeTriangle.createOn(qes);
//            Collection<com.vividsolutions.jts.triangulate.quadedge.Vertex> qeVertices = qes.getVertices(false);

//            int iV = 0;
//            for (com.vividsolutions.jts.triangulate.quadedge.Vertex v : qeVertices)
//            {
//                this.coordinates.put(v.getCoordinate(), iV);
//                this.vertices.put(iV, new Vertex(iV, v.getCoordinate()));
//                iV++;
//            }

//            // border
//            List<QuadEdge> qeFrameBorder = new ArrayList<QuadEdge>();
//            List<QuadEdge> qeFrame = new ArrayList<QuadEdge>();
//            List<QuadEdge> qeBorder = new ArrayList<QuadEdge>();

//            for (QuadEdge qe : quadEdges)
//            {
//                if (qes.isFrameBorderEdge(qe))
//                {
//                    qeFrameBorder.add(qe);
//                }
//                if (qes.isFrameEdge(qe))
//                {
//                    qeFrame.add(qe);
//                }
//            }

//            // border
//            for (int j = 0; j < qeFrameBorder.size(); j++)
//            {
//                QuadEdge q = qeFrameBorder.get(j);
//                if (!qeFrame.contains(q))
//                {
//                    qeBorder.add(q);
//                }
//            }

//            // deletion of exterior edges
//            for (QuadEdge qe : qeFrame)
//            {
//                qes.delete(qe);
//            }

//            Dictionary<QuadEdge, Double> qeDistances = new Dictionary<QuadEdge, Double>();
//            for (QuadEdge qe : quadEdges)
//            {
//                qeDistances.put(qe, qe.toLineSegment().getLength());
//            }

//            DoubleComparator dc = new DoubleComparator(qeDistances);
//            TreeMap<QuadEdge, Double> qeSorted = new TreeMap<QuadEdge, Double>(dc);
//            qeSorted.putAll(qeDistances);

//            // edges creation
//            int i = 0;
//            for (QuadEdge qe : qeSorted.keySet())
//            {
//                LineSegment s = qe.toLineSegment();
//                s.normalize();

//                int idS = this.coordinates.get(s.p0);
//                int idD = this.coordinates.get(s.p1);
//                Vertex oV = this.vertices.get(idS);
//                Vertex eV = this.vertices.get(idD);

//                Edge edge;
//                if (qeBorder.contains(qe))
//                {
//                    oV.setBorder(true);
//                    eV.setBorder(true);
//                    edge = new Edge(i, s, oV, eV, true);
//                    if (s.getLength() < this.threshold)
//                    {
//                        this.shortLengths.put(i, edge);
//                    }
//                    else
//                    {
//                        this.lengths.put(i, edge);
//                    }
//                }
//                else
//                {
//                    edge = new Edge(i, s, oV, eV, false);
//                }
//                this.edges.put(i, edge);
//                this.segments.put(s, i);
//                i++;
//            }

//            // hm of linesegment and hm of edges // with id as key
//            // hm of triangles using hm of ls and connection with hm of edges

//            i = 0;
//            for (QuadEdgeTriangle qet : qeTriangles)
//            {
//                LineSegment sA = qet.getEdge(0).toLineSegment();
//                LineSegment sB = qet.getEdge(1).toLineSegment();
//                LineSegment sC = qet.getEdge(2).toLineSegment();
//                sA.normalize();
//                sB.normalize();
//                sC.normalize();

//                Edge edgeA = this.edges.get(this.segments.get(sA));
//                Edge edgeB = this.edges.get(this.segments.get(sB));
//                Edge edgeC = this.edges.get(this.segments.get(sC));

//                Triangle triangle = new Triangle(i, qet.isBorder() ? true : false);
//                triangle.addEdge(edgeA);
//                triangle.addEdge(edgeB);
//                triangle.addEdge(edgeC);

//                edgeA.addTriangle(triangle);
//                edgeB.addTriangle(triangle);
//                edgeC.addTriangle(triangle);

//                this.triangles.put(i, triangle);
//                i++;
//            }

//            // add triangle neighbourood
//            for (Edge edge : this.edges.values())
//            {
//                if (edge.getTriangles().size() != 1)
//                {
//                    Triangle tA = edge.getTriangles().get(0);
//                    Triangle tB = edge.getTriangles().get(1);
//                    tA.addNeighbour(tB);
//                    tB.addNeighbour(tA);
//                }
//            }

//            // concave hull algorithm
//            int index = 0;
//            while (index != -1)
//            {
//                index = -1;

//                Edge e = null;

//                // find the max length (smallest id so first entry)
//                int si = this.lengths.size();

//                if (si != 0)
//                {
//                    Entry<int, Edge> entry = this.lengths.firstEntry();
//                    int ind = entry.getKey();
//                    if (entry.getValue().getGeometry().getLength() > this.threshold)
//                    {
//                        index = ind;
//                        e = entry.getValue();
//                    }
//                }

//                if (index != -1)
//                {
//                    Triangle triangle = e.getTriangles().get(0);
//                    List<Triangle> neighbours = triangle.getNeighbours();
//                    // irregular triangle test
//                    if (neighbours.size() == 1)
//                    {
//                        this.shortLengths.put(e.getId(), e);
//                        this.lengths.remove(e.getId());
//                    }
//                    else
//                    {
//                        Edge e0 = triangle.getEdges().get(0);
//                        Edge e1 = triangle.getEdges().get(1);
//                        // test if all the vertices are on the border
//                        if (e0.getOV().isBorder() && e0.getEV().isBorder() && e1.getOV().isBorder() && e1.getEV().isBorder())
//                        {
//                            this.shortLengths.put(e.getId(), e);
//                            this.lengths.remove(e.getId());
//                        }
//                        else
//                        {
//                            // management of triangles
//                            Triangle tA = neighbours.get(0);
//                            Triangle tB = neighbours.get(1);
//                            tA.setBorder(true); // FIXME not necessarily useful
//                            tB.setBorder(true); // FIXME not necessarily useful
//                            this.triangles.remove(triangle.getId());
//                            tA.removeNeighbour(triangle);
//                            tB.removeNeighbour(triangle);

//                            // new edges
//                            List<Edge> ee = triangle.getEdges();
//                            Edge eA = ee.get(0);
//                            Edge eB = ee.get(1);
//                            Edge eC = ee.get(2);

//                            if (eA.isBorder())
//                            {
//                                this.edges.remove(eA.getId());
//                                eB.setBorder(true);
//                                eB.getOV().setBorder(true);
//                                eB.getEV().setBorder(true);
//                                eC.setBorder(true);
//                                eC.getOV().setBorder(true);
//                                eC.getEV().setBorder(true);

//                                // clean the relationships with the triangle
//                                eB.removeTriangle(triangle);
//                                eC.removeTriangle(triangle);

//                                if (eB.getGeometry().getLength() < this.threshold)
//                                {
//                                    this.shortLengths.put(eB.getId(), eB);
//                                }
//                                else
//                                {
//                                    this.lengths.put(eB.getId(), eB);
//                                }
//                                if (eC.getGeometry().getLength() < this.threshold)
//                                {
//                                    this.shortLengths.put(eC.getId(), eC);
//                                }
//                                else
//                                {
//                                    this.lengths.put(eC.getId(), eC);
//                                }
//                                this.lengths.remove(eA.getId());
//                            }
//                            else if (eB.isBorder())
//                            {
//                                this.edges.remove(eB.getId());
//                                eA.setBorder(true);
//                                eA.getOV().setBorder(true);
//                                eA.getEV().setBorder(true);
//                                eC.setBorder(true);
//                                eC.getOV().setBorder(true);
//                                eC.getEV().setBorder(true);

//                                // clean the relationships with the triangle
//                                eA.removeTriangle(triangle);
//                                eC.removeTriangle(triangle);

//                                if (eA.getGeometry().getLength() < this.threshold)
//                                {
//                                    this.shortLengths.put(eA.getId(), eA);
//                                }
//                                else
//                                {
//                                    this.lengths.put(eA.getId(), eA);
//                                }
//                                if (eC.getGeometry().getLength() < this.threshold)
//                                {
//                                    this.shortLengths.put(eC.getId(), eC);
//                                }
//                                else
//                                {
//                                    this.lengths.put(eC.getId(), eC);
//                                }
//                                this.lengths.remove(eB.getId());
//                            }
//                            else
//                            {
//                                this.edges.remove(eC.getId());
//                                eA.setBorder(true);
//                                eA.getOV().setBorder(true);
//                                eA.getEV().setBorder(true);
//                                eB.setBorder(true);
//                                eB.getOV().setBorder(true);
//                                eB.getEV().setBorder(true);
//                                // clean the relationships with the triangle
//                                eA.removeTriangle(triangle);
//                                eB.removeTriangle(triangle);

//                                if (eA.getGeometry().getLength() < this.threshold)
//                                {
//                                    this.shortLengths.put(eA.getId(), eA);
//                                }
//                                else
//                                {
//                                    this.lengths.put(eA.getId(), eA);
//                                }
//                                if (eB.getGeometry().getLength() < this.threshold)
//                                {
//                                    this.shortLengths.put(eB.getId(), eB);
//                                }
//                                else
//                                {
//                                    this.lengths.put(eB.getId(), eB);
//                                }
//                                this.lengths.remove(eC.getId());
//                            }
//                        }
//                    }
//                }
//            }

//            // concave hull creation
//            List<LineString> edges = new ArrayList<LineString>();
//            for (Edge e : this.lengths.values())
//            {
//                LineString l = e.getGeometry().toGeometry(this.geomFactory);
//                edges.add(l);
//            }

//            for (Edge e : this.shortLengths.values())
//            {
//                LineString l = e.getGeometry().toGeometry(this.geomFactory);
//                edges.add(l);
//            }

//            // merge
//            LineMerger lineMerger = new LineMerger();
//            lineMerger.add(edges);
//            LineString merge = (LineString)lineMerger.getMergedLineStrings().iterator().next();

//            if (merge.isRing())
//            {
//                LinearRing lr = new LinearRing(merge.getCoordinateSequence(), this.geomFactory);
//                Polygon concaveHull = new Polygon(lr, null, this.geomFactory);
//                return concaveHull;
//            }

//            return merge;
//        }

//    }
//}