using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float max_vel = 1.5f;
    Vector2 accel, vel;
    Vector2 gravity = new Vector2(0, -2f);
    float friction = 0.95f;

    int thruster_anim_index = 0;

    public GameObject bullet;
    public Transform bullet_spawn;

    public ParticleSystem left_wing_brake, right_wing_brake;
    public TrailRenderer left_wing_trail, right_wing_trail;

    public SpriteRenderer thruster;
    public Color[] thruster_colors; // 0th element is base color

    Vector2 cam_tl, cam_br;

    float player_width = 3.5f;

    void Start()
    {
        cam_tl = Camera.main.ViewportToWorldPoint(new Vector2(0, 1));
        cam_br = Camera.main.ViewportToWorldPoint(new Vector2(1, 0));
    }

    // TODO
    // Mario can do 7 million tricks with a joystick and a single button
    // Why can't we?
    //
    void Update()
    {
        float raw_hor = Input.GetAxisRaw("Horizontal");
        float raw_vert = Input.GetAxisRaw("Vertical");

        float hor = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        float thrust = Input.GetAxis("Thrust");
        float brake = Input.GetAxis("Brake");

        thrust *= 6f;
        friction = 0.99f - (0.05f * brake);
        gravity = new Vector2(0, -2f) * (brake + 0.1f);

        if(brake > 0)
        {
            left_wing_brake.Emit(1);
            right_wing_brake.Emit(1);
        }

        if ( raw_hor != 0 || raw_vert != 0 ) // Rotate towards player input (don't rotate without input)
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(vert, hor) * Mathf.Rad2Deg);
        }
        else
        {
            // Use forward direction of the plane as the dir to apply thrust towards
            hor = Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
            vert = Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad);
        }

        if ( thrust > 0 )
        {
            thruster_anim_index++;
            if ( thruster_anim_index > thruster_colors.Length - 1 )
            {
                thruster_anim_index = 1;
            }
        }
        else
        {
            thruster_anim_index = 0;
        }
        thruster.color = thruster_colors[thruster_anim_index];

        accel += new Vector2(hor, vert) * thrust;
        accel += gravity;
        accel *= Time.deltaTime;

        vel += accel;
        vel *= friction;

        vel.x = Mathf.Clamp(vel.x, -max_vel, max_vel);
        vel.y = Mathf.Clamp(vel.y, -max_vel, max_vel);

        transform.position += (Vector3)vel;

        
        // Screen wrapping
        Vector2 pos = transform.position;

        if(pos.x > cam_br.x + player_width)
        {
            transform.position = new Vector3(cam_tl.x, pos.y);
            left_wing_trail.Clear();
            right_wing_trail.Clear();
        }
        else if(pos.x < cam_tl.x - player_width)
        {
            transform.position = new Vector3(cam_br.x, pos.y);
            left_wing_trail.Clear();
            right_wing_trail.Clear();
        }
        else if (pos.y > cam_tl.y + player_width)
        {
            transform.position = new Vector3(pos.x, cam_br.y);
            left_wing_trail.Clear();
            right_wing_trail.Clear();
        }
        else if (pos.y < cam_br.y - player_width)
        {
            transform.position = new Vector3(pos.x, cam_tl.y);
            left_wing_trail.Clear();
            right_wing_trail.Clear();
        }


        bool shoot = Input.GetButtonDown("Fire1");

        if(shoot)
        {
            Projectile proj = GameObject.Instantiate(bullet, bullet_spawn.position, Quaternion.identity).GetComponent<Projectile>();

            Vector2 dir = (bullet_spawn.position - transform.position).normalized;
            proj.Spawn(dir, vel);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // You died
        transform.position = Vector3.zero;
        accel = vel = Vector2.zero;
        left_wing_trail.Clear();
        right_wing_trail.Clear();
    }

    void OnGUI()
    {
        float hor = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        GUILayout.Box(string.Format("Accel:{0}\nVel:{1}\nPos:{2}", accel, vel, transform.position));

        float trigger = Input.GetAxis("Thrust");
        GUILayout.Box(string.Format("Input:({0:0.00000}, {1:0.00000})\nThrust:{2:0.0000}", hor, vert, trigger));
    }
}