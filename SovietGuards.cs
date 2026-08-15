using System.Collections;
using System.IO;
using UnityEngine;
using MelonLoader;
using GHPC.Effects;
using GHPC.State;
using GHPC.Vehicle;
using GHPC.Infantry;
using GHPC.Utility;


namespace SovietGuards{

    public class AlreadyConverted : MonoBehaviour
    {
        void Awake()
        {
            enabled = false;
        }
    }
    public class SovietGuardsClass : MelonMod
    {
        public static GameObject gameManager;
        public static Material guards_mat = null;
        public static MelonPreferences_Entry<bool> hide_nets;
        public static MelonPreferences_Entry<bool> mute_logging;
        public static MelonPreferences_Entry<bool> BMP1s;
        public static MelonPreferences_Entry<bool> BMP2;
        public static MelonPreferences_Entry<bool> T62;
        public static MelonPreferences_Entry<bool> T64s;
        public static MelonPreferences_Entry<bool> T80;
        public static MelonPreferences_Entry<bool> BTR60;
        public static MelonPreferences_Entry<bool> BTR70;
        public static MelonPreferences_Entry<bool> BRDM;
        public static MelonPreferences_Entry<bool> Infantry;

        private bool menuProps = false;
        Vector2 newSize = new Vector2(8, 4);

        public override void OnInitializeMelon()
        {
            MelonPreferences_Category cfg = MelonPreferences.CreateCategory("SovietGuards");
            hide_nets = cfg.CreateEntry<bool>("Remove turret camo nets", false);
            hide_nets.Description = "Removes camo nets that might obscure the Guards emblem";

            mute_logging = cfg.CreateEntry<bool>("Mute console logging", false);
            mute_logging.Description = "Silences the mod's messages in the MelonLoader console";

            BMP1s = cfg.CreateEntry<bool>("Modify BMP1s", true);
            BMP1s.Description = "Set true to modify BMP1s, false to exclude them";
            BMP2 = cfg.CreateEntry<bool>("Modify BMP2s", true);
            BMP2.Description = "Set true to modify BMP2s, false to exclude them";
            T62 = cfg.CreateEntry<bool>("Modify T62s", true);
            T62.Description = "Set true to modify T62s, false to exclude them";
            T64s = cfg.CreateEntry<bool>("Modify T64s", true);
            T64s.Description = "Set true to modify T64s, false to exclude them";
            T80 = cfg.CreateEntry<bool>("Modify T80s", true);
            T80.Description = "Set true to modify T80s, false to exclude them";
            BTR60 = cfg.CreateEntry<bool>("Modify BTR60s", true);
            BTR60.Description = "Set true to modify BTR60s, false to exclude them";
            BTR70 = cfg.CreateEntry<bool>("Modify BTR70s", true);
            BTR70.Description = "Set true to modify BTR70s, false to exclude them";
            BRDM = cfg.CreateEntry<bool>("Modify BRDMs", true);
            BRDM.Description = "Set true to modify BRDMs, false to exclude them";
            Infantry = cfg.CreateEntry<bool>("Modify Infantry", true);
            Infantry.Description = "Set true to modify infantrymen, false to exclude them";            
        }        
        
        public void NewQuad(GameObject go, Material mat)
        {
            MeshFilter filter = go.AddComponent<MeshFilter>();
            MeshRenderer render = go.AddComponent<MeshRenderer>();
            filter.mesh = new Mesh();
            filter.mesh.vertices = new Vector3[] {
                            new Vector3(1f, 0 , 1f), new Vector3(1f, 0, -1f), new Vector3(-1f, 0, 1f), new Vector3(-1f, 0, -1f) };
            filter.mesh.uv = new Vector2[] {
                            new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) };
            filter.mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
            filter.mesh.RecalculateNormals();
            render.material = mat;
        }

        public void FetchTex()
        {
            MelonLogger.Msg("Fetching textures");
            guards_mat = new Material(Shader.Find("ghpc_roundel"));
            guards_mat.shaderKeywords = new string[] { "_ALPHATEST_ON" };
            Texture2D badge = new Texture2D(128, 128, TextureFormat.DXT5, true, false);
            Texture2D scorch = new Texture2D(1024, 1024, TextureFormat.DXT5, true, false);
            try
            {
                byte[] data = File.ReadAllBytes("Mods/SovietGuards/gvardiya.png");
                badge.LoadImage(data, true);
                byte[] data2 = File.ReadAllBytes("Mods/SovietGuards/burnt.png");
                scorch.LoadImage(data2, true);

                guards_mat.SetTexture("_colour", badge);
                guards_mat.SetTexture("_burnttexture", scorch);
                guards_mat.SetFloat("_Cutoff", 0.84f);
                guards_mat.SetFloat("_colourfade", 0.195f);
                guards_mat.SetFloat("_SrcBlend", 1f);
                guards_mat.SetFloat("_Smoothness", 0.399f);
            }
            catch (FileNotFoundException e) { MelonLogger.Error(e); }
        }
        public void MenuProps()
        {            
            if (guards_mat == null || guards_mat.GetTexture("_colour") == null) 
            {
                FetchTex();
            }
            //since the prop vehicles in the scene have no 'Vehicle' component, and the BTR70 is not even tagged 'vehicle',
            //we fetch all the gameobjects in a big-ass array and filter them by their names
            GameObject[] props = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var prop in props) {                
                if (prop.name.Length < 6) { continue; }
                string short_name = prop.name.Substring(0, 6);                
                switch (short_name)
                {
                    case "T64A S":
                        if (!T64s.Value) { continue; }
                        Transform tac1 = prop.transform.Find("---T64A_MESH---/HULL/TURRET/T64A_markings/tac marker");
                        Transform tac2 = prop.transform.Find("---T64A_MESH---/HULL/TURRET/luna_elbow_3/spotlight cover/tac marker001");
                        if (tac1 != null)
                        {
                            MeshRenderer markings1_mr = tac1.GetComponent<MeshRenderer>();
                            markings1_mr.material = guards_mat;
                            markings1_mr.material.SetTextureScale("_colour", newSize);
                        }
                        if (tac2 != null)
                        {
                            MeshRenderer markings2_mr = tac2.GetComponent<MeshRenderer>();
                            markings2_mr.material = guards_mat;
                            markings2_mr.material.SetTextureScale("_colour", newSize);
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("T64 prop inducted into the Guards!"); }
                        break;
                    case "T62 St":
                        if (!T62.Value) { continue; }
                        Transform markings = prop.transform.Find("---T62_rig---/HULL/TURRET/T62_markings/tac_markings");
                        MeshRenderer markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;                      
                        markings_mr.material.SetTextureScale("_colour", newSize);
                        if (hide_nets.Value)
                        {
                            GameObject net = prop.transform.Find("---T62_rig---/HULL/TURRET/T62 turret net").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("T62 prop inducted into the Guards!"); }
                        break;
                    case "T80B S":
                        if (!T80.Value) { continue; }
                        markings = prop.transform.Find("T80B_rig/HULL/TURRET/searchlight/searchlight cover/tac marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.SetTextureScale("_colour", newSize);
                        if (!mute_logging.Value) { MelonLogger.Msg("T80 prop inducted into the Guards!"); }
                        break;
                    case "BMP2 S":
                        if (!BMP2.Value) { continue; }                        
                        markings = prop.transform.Find("BMP2_rig/HULL/TURRET/tactical marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.SetTextureScale("_colour", newSize);
                        if (hide_nets.Value)
                        {
                            GameObject net = prop.transform.Find("BMP2_rig/HULL/TURRET/bmp2 net turret").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("BMP2 prop inducted into the Guards!"); }
                        break;
                    case "BMP1P ":
                        if (!BMP1s.Value) { continue; }
                        markings = prop.transform.Find("BMP1_rig/HULL/TURRET/tac marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.SetTextureScale("_colour", newSize);
                        if (hide_nets.Value)
                        {
                            GameObject net = prop.transform.Find("BMP1_rig/HULL/TURRET/bmp1 net turret").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg("BMP1P prop inducted into the Guards!"); }
                        break;
                    case "BTR70 ":
                        if (!BTR70.Value) { continue; }
                        GameObject turret = prop.transform.Find("BTR70_rig/HULL/TURRET").gameObject;
                        GameObject guard_left = new GameObject("Guards_left");
                        guard_left.transform.parent = turret.transform;                        
                        guard_left.transform.position = turret.transform.position;
                        NewQuad(guard_left, guards_mat);
                        guard_left.transform.localScale = new Vector3(14f, 14f, 14f);
                        guard_left.transform.localPosition += new Vector3(-56.55f, 5.0f, -14.0f);
                        guard_left.transform.localRotation = Quaternion.Euler(new Vector3(309.0f, 70.0f, 5.1f));                        

                        GameObject guard_right = new GameObject("Guards_right");
                        guard_right.transform.parent = turret.transform;                        
                        guard_right.transform.position = turret.transform.position;
                        NewQuad(guard_right, guards_mat);
                        guard_right.transform.localScale = new Vector3(14f, 14f, 14f);
                        guard_right.transform.localPosition += new Vector3(57.55f, 5.0f, -14.0f);
                        guard_right.transform.localRotation = Quaternion.Euler(new Vector3(309.0f, -90.0f, 5.1f));                        

                        if (!mute_logging.Value) { MelonLogger.Msg("BTR70 prop inducted into the Guards!"); }
                        break;
                }
            }
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            
            if (sceneName == "MainMenu2_Scene" || sceneName == "t64_menu" || sceneName == "MainMenu2-1_Scene")
            {
                menuProps = true;
                MenuProps();
            }
            else { menuProps = false; }            
            
            gameManager = GameObject.Find("_APP_GHPC_");
            if (gameManager == null) return;

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Conversion), GameStatePriority.Medium);
        }
        private IEnumerator Conversion(GameState _)
        {
            if (menuProps == true) { yield break; }                       
            Vehicle[] list = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            if (guards_mat == null || guards_mat.GetTexture("_colour") == null)
            {                
                FetchTex();
            }

            foreach (var unit in list)
            {
                GameObject unit_go = unit.gameObject;
                if (unit_go == null) { continue; }
                if (unit_go.GetComponent<AlreadyConverted>() != null) { continue; }
                switch (unit.UniqueName) 
                {
                    case ("T62"):
                        if (!T62.Value) { continue; }
                        Transform markings = unit_go.transform.Find("---T62_rig---/HULL/TURRET/T62_markings/tac_markings");
                        MeshRenderer markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat; //replaces tactical markings with Guard badge                       
                        markings_mr.material.SetTextureScale("_colour", newSize); //new material needs adjusting to fit mesh
                        markings_mr.material.SetTextureScale("_burnttexture", newSize);
                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        if (hide_nets.Value)
                        {
                            GameObject net = unit.transform.Find("---T62_rig---/HULL/TURRET/T62 turret net").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("T64A"):   
                    case ("T64B"):                    
                    case ("T64A74"):                    
                    case ("T64A81"): 
                    case ("T64A83"):
                    case ("T64A84"):
                    case ("T64B81"):
                    case ("T64B1"):
                    case ("T64B181"):
                        if (!T64s.Value) { continue; }
                        Transform markings1 = unit_go.transform.Find("---T64A_MESH---/HULL/TURRET/T64A_markings/tac marker");
                        Transform markings2 = unit_go.transform.Find("---T64A_MESH---/HULL/TURRET/luna_elbow_3/spotlight cover/tac marker001");
                        if (markings1 != null) { 
                            MeshRenderer markings1_mr = markings1.GetComponent<MeshRenderer>();
                            markings1_mr.material = guards_mat;
                            markings1_mr.material.SetTextureScale("_colour", newSize);
                            markings1_mr.material.SetTextureScale("_burnttexture", newSize);
                        }
                        if (markings2 != null) { 
                            MeshRenderer markings2_mr = markings2.GetComponent<MeshRenderer>();
                            markings2_mr.material = guards_mat;
                            markings2_mr.material.SetTextureScale("_colour", newSize);
                            markings2_mr.material.SetTextureScale("_burnttexture", newSize);
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("T80B"):
                        if (!T80.Value) { continue; }
                        markings = unit_go.transform.Find("T80B_rig/HULL/TURRET/searchlight/searchlight cover/tac marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.SetTextureScale("_colour", newSize);
                        markings_mr.material.SetTextureScale("_burnttexture", newSize);
                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BMP1_SA"):
                    case ("BMP1P_SA"):
                        if (!BMP1s.Value) { continue; }
                        markings = unit_go.transform.Find("BMP1_rig/HULL/TURRET/tac marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.SetTextureScale("_colour", newSize);
                        markings_mr.material.SetTextureScale("_burnttexture", newSize);
                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        if (hide_nets.Value)
                        {
                            GameObject net = unit.transform.Find("BMP1_rig/HULL/TURRET/bmp1 net turret").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BMP2_SA"):
                        if (!BMP2.Value) { continue; }
                        markings = unit_go.transform.Find("BMP2_rig/HULL/TURRET/tactical marker");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.SetTextureScale("_colour", newSize);
                        markings_mr.material.SetTextureScale("_burnttexture", newSize);
                        if (hide_nets.Value)
                        {
                            GameObject net = unit.transform.Find("BMP2_rig/HULL/TURRET/bmp2 net turret").gameObject;
                            if (net != null) { net.SetActive(false); }
                        }
                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BTR60PB_SA"):                    
                        if (!BTR60.Value) { continue; }                        
                        unit_go.transform.Find("BTR_nva_markings/Object266").gameObject.SetActive(true);                          
                        markings = unit_go.transform.Find("btr60_rig/HULL/TURRET/Object266");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.SetTextureScale("_colour", new Vector2(1.05f, 1.05f)); //overwriting the DDR rondel requires a unique rescale
                        markings_mr.material.SetTextureScale("_burnttexture", new Vector2(1.05f, 1.05f));
                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BRDM2_SA"):
                        if (!BRDM.Value) { continue; }
                        unit_go.transform.Find("BRDM2_numbers/emblem").gameObject.SetActive(true);
                        markings = unit_go.transform.Find("BRDM2_rig/HULL/TURRET/emblem");
                        markings_mr = markings.GetComponent<MeshRenderer>();
                        markings_mr.material = guards_mat;
                        markings_mr.material.SetTextureScale("_colour", new Vector2(1.35f, 1.35f));
                        markings_mr.material.SetTextureScale("_burnttexture", new Vector2(1.35f, 1.35f));
                        markings_mr.material.SetTextureOffset("_colour", new Vector2(-0.1f, -0.16f));
                        markings_mr.material.SetTextureOffset("_burnttexture", new Vector2(-0.1f, -0.16f));
                        RendererMaterial markings_rm = new RendererMaterial();
                        markings_rm.Renderer = markings.GetComponent<MeshRenderer>();
                        unit.GetComponent<FlammablesManager>()._scorchRendererMaterials.Add(markings_rm);
                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("BTR70"): //BTR70 has no tactical symbols to overwrite, so we need to create new planes from scratch!                    
                        if (!BTR70.Value) { continue; }                        
                        GameObject turret;
                        turret = unit.transform.Find("BTR70_rig/HULL/TURRET").gameObject;  
                        GameObject guard_left = new GameObject("Guards_left");
                        guard_left.transform.parent = turret.transform;                        
                        guard_left.transform.position = turret.transform.position;
                        NewQuad(guard_left, guards_mat);
                        guard_left.transform.localScale = new Vector3(14f, 14f, 14f);
                        guard_left.transform.localPosition += new Vector3(-56.55f, 5.0f, -14.0f);
                        guard_left.transform.localRotation = Quaternion.Euler(new Vector3(309.0f, 70.0f, 5.1f));
                        RendererMaterial guard_left_rm = new RendererMaterial();
                        guard_left_rm.Renderer = guard_left.GetComponent<MeshRenderer>();

                        GameObject guard_right = new GameObject("Guards_right");
                        guard_right.transform.parent = turret.transform;                        
                        guard_right.transform.position = turret.transform.position;
                        NewQuad(guard_right, guards_mat);
                        guard_right.transform.localScale = new Vector3(14f, 14f, 14f);
                        guard_right.transform.localPosition += new Vector3(57.55f, 5.0f, -14.0f);
                        guard_right.transform.localRotation = Quaternion.Euler(new Vector3(309.0f, -90.0f, 5.1f));
                        RendererMaterial guard_right_rm = new RendererMaterial();
                        guard_right_rm.Renderer = guard_right.GetComponent<MeshRenderer>();

                        unit.GetComponent<FlammablesManager>()._scorchRendererMaterials.Add(guard_left_rm);
                        unit.GetComponent<FlammablesManager>()._scorchRendererMaterials.Add(guard_right_rm);

                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                    case ("T64R"):
                        if (!T64s.Value) { continue; }
                        unit.transform.Find("T64A_markings").gameObject.SetActive(true);
                        unit.transform.Find("---T64A_MESH---").gameObject.SetActive(true);
                        unit.transform.Find("---T64A_MESH---/HULL/TURRET/T64A_markings").gameObject.SetActive(true);
                        GameObject tac_luna = unit.transform.Find("---T64A_MESH---/HULL/TURRET/luna_elbow_3/tac marker001").gameObject;
                        tac_luna.transform.SetParent(unit.transform.Find("T64R_rig/HULL/TURRET/luna_elbow_3/luna cover"), true);
                        tac_luna.transform.localPosition = new Vector3(0f, 0f, 0f);
                        MeshRenderer tac_luna_mr = tac_luna.GetComponent<MeshRenderer>();
                        tac_luna_mr.material = guards_mat;
                        tac_luna_mr.material.SetTextureScale("_colour", newSize);
                        tac_luna_mr.material.SetTextureScale("_burnttexture", newSize);
                        unit.transform.Find("---T64A_MESH---").gameObject.SetActive(false);
                        if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                        unit_go.AddComponent<AlreadyConverted>();
                        break;
                } 
            }

            //Now for the infantry
            if (!Infantry.Value) { yield break; }
            InfantryUnit[] units = GameObject.FindObjectsByType<InfantryUnit>(FindObjectsSortMode.None);
            foreach (var unit in units)
            {
                GameObject unit_go = unit.gameObject;
                if (unit_go == null) { continue; }
                if (unit_go.GetComponent<AlreadyConverted>() != null) { continue; }
                if (unit.name.StartsWith("SA Obr73")){                    
                    GameObject torso = unit.transform.Find("Troop Base/RED_OBR73_KHAKI/dress").gameObject;
                    Transform[] bones = torso.GetComponent<SkinnedMeshRenderer>().bones;
                    Transform chest = null;
                    foreach (var bone in bones)
                    {
                        if (bone.name == "soldierChest") { chest = bone; }                        
                    }                    
                    GameObject guard = new GameObject("Guards_badge");
                    guard.transform.parent = chest;                    
                    guard.transform.position = chest.transform.position;
                    NewQuad(guard, guards_mat);
                    guard.transform.localScale = new Vector3(0.023f, 0.023f, 0.023f);
                    guard.transform.localPosition += new Vector3(-0.03f, 0.13f, -0.07f);
                    guard.transform.localRotation = Quaternion.Euler(new Vector3(20f, -90f, 5f));
                    if (!mute_logging.Value) { MelonLogger.Msg(unit.name + " inducted into the Guards!"); }
                    unit_go.AddComponent<AlreadyConverted>();
                }
            }
        }        
    }
}
