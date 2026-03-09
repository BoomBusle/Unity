using UnityEngine;
using UnityEditor;

public class ObstacleCourseBuilder : EditorWindow
{
    [MenuItem("Tools/Завдання 1 - Доміно")]
    static void BuildDomino()
    {
        if (!EditorUtility.DisplayDialog("Доміно",
            "Створити сцену з ефектом доміно?", "Створити", "Скасувати"))
            return;

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor_Domino";
        floor.transform.position = new Vector3(0f, -0.5f, 0f);
        floor.transform.localScale = new Vector3(20f, 1f, 6f);
        floor.isStatic = true;
        SetColor(floor, new Color(0.35f, 0.35f, 0.35f));
        Undo.RegisterCreatedObjectUndo(floor, "Create Floor");

        GameObject dominoParent = new GameObject("Dominos");
        Undo.RegisterCreatedObjectUndo(dominoParent, "Create Dominos");

        int count = 12;
        float spacing = 1.2f;
        float startX = -(count - 1) * spacing / 2f;

        for (int i = 0; i < count; i++)
        {
            GameObject domino = GameObject.CreatePrimitive(PrimitiveType.Cube);
            domino.name = "Domino_" + (i + 1);
            domino.transform.position = new Vector3(startX + i * spacing, 1.5f, 0f);
            domino.transform.localScale = new Vector3(0.3f, 2.5f, 1f);
            domino.transform.SetParent(dominoParent.transform);

            Rigidbody rb = domino.AddComponent<Rigidbody>();
            rb.mass = 1f;

            SetColor(domino, Color.HSVToRGB((float)i / count, 0.7f, 0.9f));
        }

        GameObject trigger = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        trigger.name = "TriggerBall";
        trigger.transform.position = new Vector3(startX - 0.5f, 4f, 0f);
        trigger.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        Rigidbody triggerRb = trigger.AddComponent<Rigidbody>();
        triggerRb.mass = 2f;
        SetColor(trigger, new Color(1f, 0.2f, 0.2f));
        Undo.RegisterCreatedObjectUndo(trigger, "Create Trigger");

        EditorUtility.DisplayDialog("Готово!", "Доміно сцену створено.\nНатисни Play — куля впаде і запустить ефект доміно.", "OK");
    }

    [MenuItem("Tools/Завдання 2 - Полоса перешкод (Варіант 16 ACF)")]
    static void BuildObstacles()
    {
        if (!EditorUtility.DisplayDialog("Полоса перешкод",
            "Створити перешкоди A, C, F?", "Створити", "Скасувати"))
            return;

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor_Obstacles";
        floor.transform.position = new Vector3(0f, -0.5f, 15f);
        floor.transform.localScale = new Vector3(30f, 1f, 50f);
        floor.isStatic = true;
        SetColor(floor, new Color(0.35f, 0.35f, 0.35f));
        Undo.RegisterCreatedObjectUndo(floor, "Create Floor");

        GameObject obstA = new GameObject("Obstacle_A_MovingPlatform");
        Undo.RegisterCreatedObjectUndo(obstA, "Create Obstacle A");

        GameObject platformA = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platformA.name = "Platform_A";
        platformA.transform.position = new Vector3(-5f, 0.5f, 5f);
        platformA.transform.localScale = new Vector3(3f, 0.4f, 2f);
        platformA.transform.SetParent(obstA.transform);
        MovingPlatform mp = platformA.AddComponent<MovingPlatform>();
        mp.pointA = new Vector3(-5f, 0.5f, 5f);
        mp.pointB = new Vector3(5f, 0.5f, 5f);
        mp.speed = 3f;
        SetColor(platformA, new Color(1f, 0.6f, 0f));

        CreateLabel("Label_A", "A: Плита (відрізок AB)", new Vector3(0f, 3f, 5f), obstA.transform);

        GameObject obstC = new GameObject("Obstacle_C_Pendulum");
        Undo.RegisterCreatedObjectUndo(obstC, "Create Obstacle C");

        CreateWall("PillarLeft_C", new Vector3(-2f, 2.5f, 15f), new Vector3(0.3f, 5f, 0.3f), obstC.transform);
        CreateWall("PillarRight_C", new Vector3(2f, 2.5f, 15f), new Vector3(0.3f, 5f, 0.3f), obstC.transform);
        CreateWall("TopBar_C", new Vector3(0f, 5.2f, 15f), new Vector3(4.5f, 0.3f, 0.3f), obstC.transform);

        GameObject pendulum = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pendulum.name = "Pendulum_C";
        pendulum.transform.position = new Vector3(0f, 2f, 15f);
        pendulum.transform.localScale = new Vector3(0.5f, 4f, 0.5f);
        pendulum.transform.SetParent(obstC.transform);
        PendulumObstacle po = pendulum.AddComponent<PendulumObstacle>();
        po.maxAngle = 50f;
        po.swingSpeed = 2.5f;
        po.armLength = 3f;
        SetColor(pendulum, new Color(0.8f, 0.2f, 0.2f));

        CreateLabel("Label_C", "C: Маятник", new Vector3(0f, 6.5f, 15f), obstC.transform);

        GameObject obstF = new GameObject("Obstacle_F_AstroidCylinder");
        Undo.RegisterCreatedObjectUndo(obstF, "Create Obstacle F");

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.name = "Cylinder_F";
        cylinder.transform.position = new Vector3(4f, 1f, 27f);
        cylinder.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        cylinder.transform.SetParent(obstF.transform);
        AstroidCylinder ac = cylinder.AddComponent<AstroidCylinder>();
        ac.radius = 4f;
        ac.speed = 1.5f;
        ac.centerOffset = new Vector3(0f, 1f, 27f);
        SetColor(cylinder, new Color(0.9f, 0.2f, 0.9f));

        CreateLabel("Label_F", "F: Циліндр (астроїда)", new Vector3(0f, 4f, 27f), obstF.transform);

        if (Object.FindObjectOfType<Light>() == null)
        {
            GameObject light = new GameObject("DirectionalLight");
            Light l = light.AddComponent<Light>();
            l.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Undo.RegisterCreatedObjectUndo(light, "Create Light");
        }

        EditorUtility.DisplayDialog("Готово!", "Полосу перешкод створено.\nПерешкоди: A (плита), C (маятник), F (циліндр-астроїда).\nНатисни Play для перегляду.", "OK");
    }

    static void SetColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            Material mat = new Material(shader);
            mat.SetColor("_BaseColor", color);
            mat.color = color;
            renderer.sharedMaterial = mat;
        }
    }

    static void CreateWall(string name, Vector3 pos, Vector3 scale, Transform parent)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.transform.SetParent(parent);
        wall.isStatic = true;
        SetColor(wall, new Color(0.5f, 0.5f, 0.5f));
    }

    static void CreateLabel(string name, string text, Vector3 pos, Transform parent)
    {
        GameObject label = new GameObject(name);
        label.transform.position = pos;
        label.transform.SetParent(parent);

        TextMesh tm = label.AddComponent<TextMesh>();
        tm.text = text;
        tm.fontSize = 32;
        tm.characterSize = 0.15f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = Color.white;
    }
}
