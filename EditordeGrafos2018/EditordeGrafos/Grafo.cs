﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EditordeGrafos{
[Serializable()]

public class Grafo:List<NodoP>{
    private bool edgeNamesVisible;
    private bool edgeWeightVisible;
    private int nodeRadio;
    private int nodeBorderWidth;
    private int edgeWidth;
    private int[][] matriz;
    private Color edgeColor;
    private Color nodeColor;
    private Color nodeBorderColor;
    private List<Edge> edgesList;

    #region get's & set's

    public int EdgeWidth {
        get { return edgeWidth; }
        set { edgeWidth = value; }
    }

    public Color NodeBorderColor {
        get { return nodeBorderColor; }
        set { nodeBorderColor = value; }
    }

    public int NodeBorderWidth{
        get { return nodeBorderWidth; }
        set { nodeBorderWidth = value; }
    }

    public Color EdgeColor{ 
        get { return edgeColor; } 
        set { edgeColor = value; } 
    }

    public Color NodeColor {
        get { return nodeColor; }
        set { nodeColor = value; } 
    }
        
    public List<Edge> EdgesList { 
        get { return edgesList; } 
    }
        
    public int[][] Matriz{
        get { return matriz; } 
    }
        
    public int NodeRadio{
        get { return nodeRadio;} 
        set { nodeRadio = value; } 
    }
        
    public bool EdgeNamesVisible{
        get { return edgeNamesVisible; } 
        set { edgeNamesVisible = value; } 
    }
        
    public bool EdgeWeightVisible{
        get { return edgeWeightVisible; } 
        set { edgeWeightVisible = value; } 
    }

    #endregion
    #region constructores

    public Grafo(){
        edgesList = new List<Edge>();
        edgeColor = Color.Black;
        edgeNamesVisible = false;
        edgeWeightVisible = false;
        nodeColor = Color.White;
        nodeRadio = 30;
        nodeBorderWidth = 1;
        edgeWidth = 1;
        nodeBorderColor = Color.Black;
    }

    public Grafo(Grafo gr){

        edgesList = new List<Edge>();
        edgeColor = gr.EdgeColor;
        nodeColor = gr.nodeColor;
        NodoP aux1,aux2;
        nodeRadio = gr.NodeRadio;
        Edge k = new Edge();
        nodeBorderWidth = 1;
        edgeWidth = 1;
        nodeBorderColor = Color.Black;
        edgeNamesVisible = gr.EdgeNamesVisible;
        edgeWeightVisible = gr.EdgeWeightVisible;
            

        foreach (NodoP n in gr){
            this.Add(new NodoP(n));
        }

        foreach(NodoP n in gr){
            aux1 = Find(delegate(NodoP bu) { if (bu.Name == n.Name)return true; else return false; });
            foreach (NodoRel rel in n.relations){
                aux2 = Find(delegate(NodoP je) { if (je.Name == rel.Up.Name)return true; else return false; });
                aux1.InsertRelation(aux2, EdgesList.Count);
            }
        }

        foreach (Edge ar in gr.edgesList){
            aux1 = Find(delegate(NodoP bu) { if (bu.Name == ar.Origin.Name)return true; else return false; });
            aux2 = Find(delegate(NodoP bu) { if (bu.Name == ar.Destiny.Name)return true; else return false; });
            k = new Edge(ar.Type, aux1, aux2, ar.Name);
            k.Weight = ar.Weight;
            AddEdge(k);
        }
    }

    #endregion
    #region operaciones

    public void AgregaNodo(NodoP n){ 
        Add(n);
    }

    public void AddEdge(Edge A){
        edgesList.Add(A);
    }

    public void RemueveArista(Edge ar){
        NodoRel rel;
            
        rel = ar.Origin.relations.Find(delegate(NodoRel np) { if (np.Up.Name==ar.Destiny.Name)return true; else return false; });
        if (rel != null){
            ar.Origin.relations.Remove(rel);
            ar.Origin.Degree--;
            ar.Destiny.Degree--;
            ar.Origin.DegreeEx--;
            ar.Destiny.DegreeIn--;
        }
        if (ar.Type == 2){
            rel = ar.Destiny.relations.Find(delegate(NodoRel np) { if (np.Up.Name==ar.Origin.Name)return true; else return false; });
                
            if (rel != null){
                ar.Destiny.relations.Remove(rel);
                ar.Destiny.DegreeEx--;
                ar.Origin.DegreeIn--;
            }
        }
        edgesList.Remove(ar);
    }
    public void RemueveNodo(NodoP rem){
        NodoRel rel;
        List<Edge>remover;
        remover=new List<Edge>();
            
        foreach(NodoP a in this){
            rel = a.relations.Find(delegate(NodoRel np) { if (np.Up.Name==rem.Name)return true; else return false; });
            if (rel != null){
                a.relations.Remove(rel);
                a.Degree--;
                a.DegreeEx--;
                if(edgesList[0].Type == 0 || edgesList[0].Type == 2){
                    a.DegreeIn--;
                }
            }
        }
        remover=edgesList.FindAll(delegate(Edge ar){if(ar.Origin.Name==rem.Name||ar.Destiny.Name==rem.Name)return true;else return false;});
        if(remover!=null)
            foreach(Edge re in remover){
                edgesList.Remove(re);
            }
        this.Remove(rem);
    }

    #endregion
    #region paint

    public void pinta(Graphics g)
    {
        Pen pendi = new Pen(edgeColor, edgeWidth);
        Pen penNdi = new Pen(edgeColor, edgeWidth);
        Pen pen = new Pen(nodeBorderColor, nodeBorderWidth);

        AdjustableArrowCap end=new AdjustableArrowCap(6,6);
        SolidBrush nod;
        pendi.CustomEndCap = end;
        Size s = new Size(nodeRadio, nodeRadio);
        double p3x,p3y, p4x,p4y;
        double ang;
        PointF A, B;
        A = new PointF();
        double d;
        double r;
        double an;
        //int multi;
        double dy,dx;
        dy = dx=0;
        List<Edge> repetidas = new List<Edge>();
        if(edgesList.Count > 0){
            foreach (Edge a in edgesList){
                if(a.Type != 1){
                    if(a.Origin.Name == a.Destiny.Name){
                        g.DrawBezier(penNdi, new Point(a.Origin.Position.X + ((a.Destiny.Position.X - a.Origin.Position.X) / 2) - 10, a.Origin.Position.Y + ((a.Destiny.Position.Y - a.Origin.Position.Y) / 2) - 5), new Point(a.Origin.Position.X + ((a.Destiny.Position.X - a.Origin.Position.X) / 2) - 40, a.Origin.Position.Y - ((a.Destiny.Position.Y - a.Origin.Position.Y) / 2) - 60), new Point(a.Origin.Position.X + 40, a.Destiny.Position.Y - 60), new Point(a.Destiny.Position.X + 10, a.Destiny.Position.Y - 5));
                    }
                    else{
                        g.DrawLine(penNdi, a.Origin.Position.X , a.Origin.Position.Y, a.Destiny.Position.X, a.Destiny.Position.Y);
                    }

                    repetidas = edgesList.FindAll(delegate(Edge repe) { if (repe.Origin.Name == a.Origin.Name && repe.Destiny.Name == a.Destiny.Name || (a.Origin.Name == repe.Destiny.Name && a.Destiny.Name == repe.Origin.Name))return true;else return false; });
                        
                    if(repetidas.Count > 1 && a.Painted==false){
                        if((a.Destiny.Position.Y - a.Origin.Position.Y) != 0 ){
                            g.DrawString(repetidas.Count.ToString(), new Font("Arial", 10), Brushes.Black, a.Origin.Position.X + ((a.Destiny.Position.X - a.Origin.Position.X) / 2) + 4, a.Origin.Position.Y + ((a.Destiny.Position.Y - a.Origin.Position.Y) / 2) + 4);                                foreach (Edge re in repetidas)
                            re.Painted = true;
                        }
                    }
                }
                else{
                    if(a.Origin.Name == a.Destiny.Name){
                        g.DrawBezier(pendi, new Point(a.Origin.Position.X - 10, a.Origin.Position.Y - 5), new Point(a.Origin.Position.X - 40, a.Origin.Position.Y - 60), new Point(a.Origin.Position.X + 40, a.Destiny.Position.Y - 60), new Point(a.Destiny.Position.X + 10, a.Destiny.Position.Y - 10));
                    }
                    else{
                        if(edgesList.Find(delegate(Edge bus) { if (bus.Origin.Name == a.Destiny.Name && bus.Destiny.Name == a.Origin.Name)return true; else return false; }) == null){
                            double teta1 = Math.Atan2((double)(a.Destiny.Position.Y - a.Origin.Position.Y),(double)( a.Destiny.Position.X - a.Origin.Position.X));
                            float x1 = a.Origin.Position.X + (float)((Math.Cos(teta1)) * (s.Height/2));
                            float y1 = a.Origin.Position.Y + (float)((Math.Sin(teta1)) * (s.Height / 2));

                            double teta2 = Math.Atan2(a.Origin.Position.Y - a.Destiny.Position.Y, a.Origin.Position.X - a.Destiny.Position.X);
                            float x2 = a.Destiny.Position.X + (float)((Math.Cos(teta2)) * (s.Height / 2));
                            float y2 = a.Destiny.Position.Y + (float)((Math.Sin(teta2)) * (s.Height / 2));
                            g.DrawLine(pendi, x1, y1, x2, y2);

                            if(edgesList.FindAll(delegate(Edge repe) { if (repe.Origin.Name == a.Origin.Name && repe.Destiny.Name == a.Destiny.Name)return true; else return false; }).Count > 1){
                                if((a.Destiny.Position.Y - a.Origin.Position.Y) != 0){
                                    g.DrawString(edgesList.FindAll(delegate(Edge repe) { if (repe.Origin.Name == a.Origin.Name && repe.Destiny.Name == a.Destiny.Name)return true; else return false; }).Count.ToString(), new Font("Arial", 10), Brushes.Black, a.Origin.Position.X + ((a.Destiny.Position.X - a.Origin.Position.X) / 2) + 4, a.Origin.Position.Y + ((a.Destiny.Position.Y - a.Origin.Position.Y) / 2) + 4);
                                }
                            }

                        }
                        else{
                            if(a.Painted == false){
                                dy = a.Destiny.Position.Y - a.Origin.Position.Y;
                                dx = a.Destiny.Position.X - a.Origin.Position.X;

                                p3x = (dx * 1 / 3) + a.Origin.Position.X;
                                p3y = (dy * 1 / 3) + a.Origin.Position.Y;
                                p4x = (dx * 2 / 3) + a.Origin.Position.X;
                                p4y = (dy * 2 / 3) + a.Origin.Position.Y;

                                d = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                                r = .35 * d;

                                if(a.Destiny.Position.X != a.Origin.Position.X){
                                    ang = Math.Atan((double)((double)dy / (double)dx));
                                }
                                else{
                                    ang = 90;
                                }

                                if(a.Destiny.Position.X > a.Origin.Position.X){
                                    an = ang + 89.8;
                                }
                                else{
                                    an = ang - 89.8;
                                }
                                    
                                B = new PointF((float)((r * Math.Cos(an)) + p4x), (float)((r * Math.Sin(an)) + p4y /*+ 15 * (an / Math.Abs(an))*/));
                                A = new PointF((float)((r * Math.Cos(an)) + p3x), (float)((r * Math.Sin(an)) + p3y /*+ 15 * (an / Math.Abs(an))*/));
                                    
                                if(a.Destiny.Position.X > a.Origin.Position.X){
                                    an = ang + 89.56;
                                }
                                else{
                                    an = ang - 89.56;
                                }

                                g.DrawBezier(pendi, new PointF(a.Origin.Position.X + (float)((Math.Cos(an)) * (s.Height / 2)), a.Origin.Position.Y + (float)((Math.Sin(an)) * (s.Height / 2))), A, B, new PointF(a.Destiny.Position.X + (float)((Math.Cos(an)) * (s.Height / 2)), a.Destiny.Position.Y + (float)((Math.Sin(an)) * (s.Height / 2))));
                                a.Painted = true;                   
                            }
                        }
                        if(edgesList.FindAll(delegate(Edge repe) { if (repe.Origin.Name == a.Origin.Name && repe.Destiny.Name == a.Destiny.Name)return true; else return false; }).Count > 1){
                            if((a.Destiny.Position.Y - a.Origin.Position.Y)!=0){
                                g.DrawString(edgesList.FindAll(delegate(Edge repe) { if (repe.Origin.Name == a.Origin.Name && repe.Destiny.Name == a.Destiny.Name)return true; else return false; }).Count.ToString(), new Font("Arial", 10), Brushes.Black, a.Destiny.Position.X,A.Y-10);
                            }
                        }
                    }
                }
                   
                if (edgeNamesVisible){
                    g.DrawString(a.Name, new Font("Bold", 10), Brushes.Blue, a.Origin.Position.X + ((a.Destiny.Position.X - a.Origin.Position.X) / 3) + 4, a.Origin.Position.Y + ((a.Destiny.Position.Y - a.Origin.Position.Y) / 2) + 1);
                }
                if (edgeWeightVisible){
                    if (edgesList.Find(delegate(Edge bus) { if (bus.Origin.Name == a.Destiny.Name && bus.Destiny.Name == a.Origin.Name)return true; else return false; }) == null){
                        g.DrawString(a.Weight.ToString(), new Font("Bold", 10), Brushes.Blue, a.Origin.Position.X + ((a.Destiny.Position.X - a.Origin.Position.X) / 2) + 4, a.Origin.Position.Y + ((a.Destiny.Position.Y - a.Origin.Position.Y) / 2) + 4);
                    }
                    else{
                        g.DrawString(a.Weight.ToString(), new Font("Bold", 10), Brushes.Blue, a.Destiny.Position.X, A.Y - 10);
                    }
                }
                    
            }
        }
        foreach (Edge des in edgesList){
            des.Painted = false;
        }
        foreach (NodoP n in this){
            pendi.Width = 3;
            if(n.Selected==false){
                nod = new SolidBrush(n.Color);
            }
            else{
                nod = new SolidBrush(Color.Red);
            }

            Rectangle re = new Rectangle(n.Position.X - (s.Height / 2), n.Position.Y - (s.Height / 2), s.Width, s.Height);
            g.DrawRectangle(pen, re);
            g.FillEllipse(nod, re);
            g.DrawEllipse(pen, re);
            g.DrawString(n.Name.ToString(), new Font("Bold", nodeRadio / 4), new SolidBrush(nodeBorderColor), (n.Position.X - nodeRadio/4 + nodeRadio/12), (n.Position.Y - nodeRadio/4 + nodeRadio/12));
        }
        pendi.Dispose();
        penNdi.Dispose();
        pen.Dispose();
           
    }

    #endregion
    #region algoritmos

    public void CreaMatriz(){ //
        matriz = new int[Count][];
        int i = 0, j;
            
        for (int x = 0; x < Count; x++){
            matriz[x]=new int[Count];
        }
            
        this.Sort(delegate(NodoP a, NodoP b) { 
            return a.Name.CompareTo(b.Name); 
        });

        foreach(NodoP nod in this){
            j=0;
            foreach(NodoP nod2 in this){
                if (nod.relations.Find(delegate(NodoRel r) {if(nod2.Name == r.Up.Name) return true; else return false; }) != null){
                    matriz[i][j] = 1;
                }
                else{
                    matriz[i][j] = 0;    
                }
                j++;
            }
            i++;
        }
    }
       
    public int Componentes2(NodoP nodito, List<NodoP> compo){
        if(nodito.relations.Find(delegate(NodoRel r) { if (r.Visited!=true )return true; else return false; }) == null){
            return 1;
        }
        else{
            compo.Add(nodito);
            foreach (NodoRel a in nodito.relations){
                if (a.Visited == false){
                    a.Visited = true;
                    compo.Add(a.Up);
                    Componentes2(a.Up, compo);
                }
            }
            return 0;
        }
    }

    public int Componentes(){
        bool enco = false;
        bool enco2=false;
        List<List<NodoP>> componentes = new List<List<NodoP>>();
        List<NodoP> nue = new List<NodoP>();
        Grafo aux = new Grafo(this);

        if(edgesList.Count != 0){
            foreach(NodoP nod in this){
                foreach(List<NodoP> n in componentes){
                    if(enco == false)
                        if(n.Find(delegate(NodoP f) { if (f.Name == nod.Name)return true; else return false; }) != null)
                            enco = true;
                }
                if(enco == false){
                    if(edgesList[0].Type == 1){
                        foreach(List<NodoP> n in componentes){
                            foreach(NodoP ee in n){
                                foreach(NodoRel r in ee.relations){
                                    if(enco2 == false)
                                        if (n.Find(delegate(NodoP f) { if (f.Name == r.Up.Name)return true; else return false; }) != null)
                                            enco2 = true;
                                }
                            }
                        }
                        if(enco2 == false){
                            nue = new List<NodoP>();
                            this.Componentes2(nod, nue);
                            componentes.Add(nue);
                        }
                        enco2 = false;
                    }
                    else{
                        nue = new List<NodoP>();
                        this.Componentes2(nod, nue);
                        componentes.Add(nue);
                    }
                }
                enco = false;
            }
            foreach(NodoP re in this){
                foreach(NodoRel rela in re.relations){
                    rela.Visited = false;
                }
            }
            return componentes.Count;
        }
        else{
            return this.Count;
        }
    }

    public void Deselect(){
        foreach (NodoP r in this){
            r.Selected = false;
        }
    }

    public List<List<NodoP>> colorear(){
            
        bool found = false;
        int re = 0, g = 0, b = 255;            
        Color co = Color.FromArgb(re, g, b);
        List<List<NodoP>> listas=new List<List<NodoP>>();
        List<NodoP> au = new List<NodoP>();
            
        foreach(NodoP nodin in this){
            foreach(List<NodoP> c in listas){
                if(found == false)
                    if (c.Find(delegate(NodoP a) { if (a.relations.Find(delegate(NodoRel r) { if (r.Up.Name == nodin.Name)return true; else return false; }) != null || nodin.relations.Find(delegate(NodoRel rela){if(rela.Up.Name==a.Name)return true;else return false;})!=null)return true; else return false; }) == null)
                    {
                        c.Add(nodin);
                        found = true;
                    }
            }
            if (found == false){
                au = new List<NodoP>();
                au.Add(nodin);
                listas.Add(au);
            }
            found = false;
        }
        foreach(List<NodoP> a in listas){
            foreach (NodoP n in a){
                n.Color = co;
            }
            if (re + 100 >= 255){
                re = 0;
                if (g + 100 >= 255){
                    g = 0;
                    if (b + 150 >= 255){
                        b = 0;
                    }
                    else{
                        b += 150;
                    }
                }
                else{
                    g += 100;
                }
            }
            else{
                re += 100;
                b = 180;
            }
            co = Color.FromArgb(co.R-co.R+re, co.G-co.G+g, co.B-co.B+b );      
        }
        return listas;

    }

    public Grafo Complement() { // saca el complemento del grafo
        Grafo graph2 = new Grafo(this);
        int i = 0, j = 0;
        NodoP npn;
        NodoRel rpr, aux;
        char nodName = 'A';
        List<Edge> inverse = new List<Edge>();
        foreach (NodoP np in this) { // recorre la lista principal
            nodName = 'A';
            npn = graph2.ElementAt<NodoP>(i); // obtiene el nodo del nuevo grafo
            for (j = 0; j < this.Count; j++) { // recorre todas las relaciones cada nodo del gafo principal
                aux = npn.relations.ElementAt<NodoRel>(j);

                if (aux.Name != nodName.ToString()) {
                    npn.relations.Add(aux);
                    inverse.Add(new Edge(2, np, aux.Up, aux.Name));
                }
                nodName++;
            }
            j = 0;
        }
        edgesList.Clear();
        edgesList = inverse;
        return graph2;
    }

    #endregion
}
}
   