using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [SerializeField]
    float magnitud = 5;

    new SpriteRenderer renderer;
    AudioSource[] audios;
    float[] volumes;
    Animator animator;

    int state = 0;

    void Start() {
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audios = GetComponentsInChildren<AudioSource>();

        volumes = new float[audios.Length];
        for (int i=0; i<audios.Length; i++) {
            volumes[i] = audios[i].volume;
        }
    }

    void Update() {

        if (state == 0) {
            float input = Input.GetAxis("Horizontal");
            //animator.SetFloat("Speed", Mathf.Abs(input));

            if (input != 0) { // Movement
                Vector3 speed = magnitud * Vector2.right * input;
                transform.position += speed * Time.deltaTime;

                if (input < 0) renderer.flipX = true;
                if (input > 0) renderer.flipX = false;

                PlaySound(0);
                Debug.Log(audios[0].clip.name);

                animator.speed = 0.75f + 0.25f * Mathf.Abs(input);
            } else {

                animator.speed = 1f;
                PlaySound(-1);
            }

            if (Input.GetButtonDown("Fire1")) {
                PlaySound(-1);
                animator.SetBool("Attack", true);
                state = 1;
            }
        }


        if (state == 1) {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Robot Attack")) {
                state = 2;
                PlaySound(1);
            }
        }

        if (state == 2 || state == 3) {
            float percentage = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (percentage >= 0.5f) {
                state = 3;

            }

            if (percentage >= 0.9f) {
                animator.SetBool("Attack", false);
                state = 4;
            }
        }

        if (state == 4) {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Robot Idle")) {
                state = 0;
            }
        }

    }

    void OnTriggerStay2D(Collider2D collision) {
        if (state == 3 && collision.CompareTag("Enemy")) {
            Destroy(collision.gameObject);
        }
    }

    void PlaySound(int index) {
        StopCoroutine(PlaySoundCoroutine(index));
        StartCoroutine(PlaySoundCoroutine(index));
    }

    IEnumerator PlaySoundCoroutine(int index) {

        if (0 <= index && index < audios.Length) {
            if (!audios[index].isPlaying) audios[index].Play();
            audios[index].volume = volumes[index];
        }
        float time = 0;
        float duration = 0.25f;
        while (time < duration) {
            for (int i = 0; i < audios.Length; i++) {
                if (i == index) continue;
                float volume = Mathf.Lerp(volumes[i], 0, time / duration);
                if(audios[i].volume > volume) audios[i].volume = Mathf.Lerp(volumes[i], 0,time/duration);
                yield return null;
                time += Time.deltaTime;
            }
        }
    }
}
