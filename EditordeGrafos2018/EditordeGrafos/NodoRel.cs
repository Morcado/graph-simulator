﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditordeGrafos{
    [Serializable()]

    public class NodoRel{
        private bool visited;
        private string name;
        private NodoP up;

        public string Name{ 
            get {return name;} 
            set {name = value;} 
        }
        public NodoP Up{
            get {return up;}
        }
        public bool Visited{ 
            get { return visited; } 
            set { visited = value; } 
        }
        
        public NodoRel(NodoP np, string nam){
            up = np;
            visited = false;
            name = nam;
        }
    }
}
