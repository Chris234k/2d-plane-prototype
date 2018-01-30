using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player physics
    float max_vel = 1.5f;
    Vector2 accel, vel;
    Vector2 gravity = new Vector2(0, -2f);
    float friction = 0.95f;

    // Movement animations
    public ParticleSystem left_wing_brake, right_wing_brake;
    public TrailRenderer left_wing_trail, right_wing_trail;
    
    // Shooting
    public GameObject bullet;
    public Transform bullet_spawn;

    // Thruster animations
    int thruster_anim_index = 0;
    public SpriteRenderer thruster;
    public Color[] thruster_colors; // 0th element is base color

    // Screen wrapping
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
        // # GAME FEEL - Inputs come from Unity as value from 0 - 1. We use these as percentages.
        // Where '0' means unpressed and '1' means fully pressed.
        float raw_h = Input.GetAxisRaw("Horizontal");
        float raw_v = Input.GetAxisRaw("Vertical");

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float thrust = Input.GetAxis("Thrust");
        float brake = Input.GetAxis("Brake");

        thrust *= 6f;
        friction = 0.99f - (0.05f * brake); // # GAME FEEL - Full press on trigger results in higher brake
        gravity = new Vector2(0, -2f) * (brake + 0.1f);

        if(brake > 0) // # GAME FEEL - Emit particles while braking. We always have trail renderers, but these differentiate movement and braking visually.
        {
            left_wing_brake.Emit(1);
            right_wing_brake.Emit(1);
        }

        if ( raw_h != 0 || raw_v != 0 ) // Rotate towards player input (don't rotate without input)
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(v, h) * Mathf.Rad2Deg);
        }
        else
        {
            // Use forward direction of the plane as the dir to apply thrust towards
            h = Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad);
            v = Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad);
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

        accel += new Vector2(h, v) * thrust; // # GAME FEEL - Full press on trigger results in higher thrust (same as above for braking)
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
        // Player death
        transform.position = Vector3.zero;
        accel = vel = Vector2.zero;
        left_wing_trail.Clear();
        right_wing_trail.Clear();
    }

    // Display values on screen
    // Just an easy way to see what's happening
    // If this seems weird, just make the variables public and look at them in Unity's Inspector.
    // 
    // void OnGUI()
    // {
    //     float hor = Input.GetAxis("Horizontal");
    //     float vert = Input.GetAxis("Vertical");

    //     GUILayout.Box(string.Format("Accel:{0}\nVel:{1}\nPos:{2}", accel, vel, transform.position));

    //     float trigger = Input.GetAxis("Thrust");
    //     GUILayout.Box(string.Format("Input:({0:0.00000}, {1:0.00000})\nThrust:{2:0.0000}", hor, vert, trigger));
    // }
}