using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    //Player movimentação
    public float speed = 5f;
    public float jumpforce = 600;
    

    private Rigidbody2D rb2d;
    private bool facingRight = true;
    private bool jump;
    private bool onGround = false;
    private Transform groundCheck;
    private float hForce = 0;

    private bool isDead = false;

    //Player animações
    private Animator anim;
    private bool crouched;
    private bool lookingUp;
    private bool reloading;

    //Tiro
    private float fireRate = 0.5f;
    private float nextFire;
    public GameObject bulletPreab;
    public Transform shotSpawner;

	// Use this for initialization
	void Start (){

        rb2d = GetComponent<Rigidbody2D>();
        groundCheck = gameObject.transform.Find("GroundCheck");
        anim = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!isDead){
            //Define o layer de chão
            onGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

            //Se o player estiver no chao quer dizer que pode pular
            if (onGround){
                anim.SetBool("Jump", false);
            }

            //Configuração do pulo
            if(Input.GetButtonDown("Jump") && onGround && !reloading){
                jump = true;
            }else if (Input.GetButtonUp("Jump")){
                if(rb2d.velocity.y > 0){
                    rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
                }
            }

            //Se o botão de tiro estiver pressionado, muda a animação tiro
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire){
                nextFire = Time.time + fireRate;
                anim.SetTrigger("Shoot");
                GameObject tempBullet = Instantiate(bulletPreab, shotSpawner.position, shotSpawner.rotation);

                //Se o player virar de lado, mudar o tiro e se estiver olhando rpa cima mudar o angulo do tiro
                if (!facingRight && !lookingUp){
                    tempBullet.transform.eulerAngles = new Vector3(0, 0, 180);
                }else if(!facingRight && lookingUp){
                    tempBullet.transform.eulerAngles = new Vector3(0, 0, 90);
                }

                if (crouched && !onGround){
                    tempBullet.transform.eulerAngles = new Vector3(0, 0, -90);
                }
            }

            //definindo botoes de olhar pra cima e abaixar
            lookingUp = Input.GetButton("Up");
            crouched = Input.GetButton("Down");

            //Setando animações de olhar pra cima e abaixar apos clicar no botao
            anim.SetBool("LookingUp", lookingUp);
            anim.SetBool("Crouched", crouched);

            //Se carregando, chama a animação carregar
            if (Input.GetButtonDown("Reload")){
                anim.SetBool("Reloading", true);
            }

            //Se agachado, olhando pra cima, ou reloading e estiver no chão, player fica parado
            if ((crouched || lookingUp || reloading) && onGround){
                hForce = 0;
            }

        }
		
	}

    private void FixedUpdate()
    {
        if (!isDead){
            if(!crouched && !lookingUp && !reloading)
            hForce = Input.GetAxisRaw("Horizontal");
            
            //se estiver indo pra esquerda ou direita, o math.abs transforma em positiva e seta no Speed
            anim.SetFloat("Speed", Mathf.Abs(hForce));

            rb2d.velocity = new Vector2(hForce * speed, rb2d.velocity.y);

            if(hForce > 0 && !facingRight){
                Flip();
            }else if(hForce < 0 && facingRight){
                Flip();
            }

            //Se jump for verdadeiro seta no animator, coloca false para nao pular de novo e muda a velocidade
            if (jump){
                anim.SetBool("Jump", true);
                jump = false;
                rb2d.AddForce(Vector2.up * jumpforce);
            }
        }
    }

    //Função para virar o player
    void Flip(){
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
