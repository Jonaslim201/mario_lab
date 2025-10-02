using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerData playerData;

    // private float xAxis;
    private Rigidbody2D marioBody;

    private bool onGroundState = true;
    // global variables
    private bool faceRightState = true;
    public Animator marioAnimator;
    public AudioSource marioAudio;
    public AudioClip marioDeath;
    public float deathImpulse = 35;
    [System.NonSerialized]
    public bool alive = true;
    public GameOverController gameOverController;
    public GameObject enemies;
    // private Vector3 originalPosition;
    public JumpOverGoomba jumpOverGoomba;

    #region STATE PARAMETERS
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other sctipts to read them
    //but can only be privately written to.
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsSliding { get; private set; }

    //Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    //Dash
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    private bool _isDashAttacking;

    #endregion

    #region INPUT PARAMETERS
    private Vector2 _moveInput;

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    //Set all of these up in the inspector
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        marioBody = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        setGravityScale(playerData.gravityScale);
        IsFacingRight = true;
        setGroundState(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!alive) return;

        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;

        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
        {
            CheckDirectionToFace(_moveInput.x > 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump pressed");
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Jump released");
            OnJumpUpInput();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Dash pressed");
            onDashInput();
        }

        if (!IsDashing && !IsJumping)
        {

            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                if (LastOnGroundTime < -0.1f)
                {
                    // animation on land
                    setGroundState(true);
                }
                LastOnGroundTime = playerData.coyoteTime;
            }
            else
            {
                setGroundState(false); // Clear ground state when not touching ground
            }

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
                LastOnWallRightTime = playerData.coyoteTime;

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
                LastOnWallLeftTime = playerData.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }

        if (IsJumping && marioBody.linearVelocityY < 0)
        {
            IsJumping = false;

            _isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > playerData.wallJumpTime)
        {
            Debug.Log("Wall Jump ended");
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;

            _isJumpFalling = false;
        }

        if (!IsDashing)
        {
            //Jump
            if (CanJump() && LastPressedJumpTime > 0)
            {
                IsJumping = true;
                IsWallJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;
                Jump();

                // AnimHandler.startedJumping = true;
            }
            //WALL JUMP
            else if (CanWallJump() && LastPressedJumpTime > 0)
            {
                IsWallJumping = true;
                IsJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;

                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
                Debug.Log("Wall Jump Direction: " + _lastWallJumpDir);

                WallJump(_lastWallJumpDir);
            }
        }

        #region DASH CHECKS
        if (CanDash() && LastPressedDashTime > 0)
        {
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            Sleep(playerData.dashSleepTime);

            //If not direction pressed, dash forward
            if (_moveInput != Vector2.zero)
                _lastDashDir = _moveInput;
            else
                _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



            IsDashing = true;
            IsJumping = false;
            IsWallJumping = false;
            _isJumpCut = false;

            StartCoroutine(nameof(StartDash), _lastDashDir);
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
            IsSliding = true;
        else
            IsSliding = false;
        #endregion

        #region GRAVITY
        if (!_isDashAttacking)
        {
            //Higher gravity if we've released the jump input or are falling
            if (IsSliding)
            {
                setGravityScale(0);
            }
            else if (marioBody.linearVelocityY < 0 && _moveInput.y < 0)
            {
                //Much higher gravity if holding down
                setGravityScale(playerData.gravityScale * playerData.fastFallGravityMultiplier);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocityX, Mathf.Max(marioBody.linearVelocityY, -playerData.maxFastFallSpeed));
            }
            else if (_isJumpCut)
            {
                //Higher gravity if jump button released
                setGravityScale(playerData.gravityScale * playerData.jumpCutGravityMultiplier);
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, Mathf.Max(marioBody.linearVelocity.y, -playerData.maxFallSpeed));
            }
            else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(marioBody.linearVelocity.y) < playerData.jumpHangTimeThreshold)
            {
                setGravityScale(playerData.gravityScale * playerData.jumpHangGravityMultiplier);
            }
            else if (marioBody.linearVelocity.y < 0)
            {
                //Higher gravity if falling
                setGravityScale(playerData.gravityScale * playerData.fallGravityMultiplier);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, Mathf.Max(marioBody.linearVelocity.y, -playerData.maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                setGravityScale(playerData.gravityScale);
            }
        }
        else
        {
            //No gravity when dashing (returns to normal once initial dashAttack phase over)
            setGravityScale(0);
        }
        #endregion
    }
    void FixedUpdate()
    {
        if (!alive) return;
        //Handle Run
        if (!IsDashing)
        {
            if (IsWallJumping)
                Run(playerData.wallJumpRunLerp);
            else
                Run(1);
        }
        else if (_isDashAttacking)
        {
            Run(playerData.dashEndRunLerp);
        }

        //Handle Slide
        if (IsSliding)
            Slide();
    }

    // Flip the character to face the mouse cursor (for future use with shooting)
    private void FlipController()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < transform.position.x && faceRightState)
        {
            Flip();
        }
        else if (mousePos.x > transform.position.x && !faceRightState)
        {
            Flip();
        }
    }

    private void Flip()
    {

        faceRightState = !faceRightState;
        transform.Rotate(0, 180, 0);

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            Debug.Log("Game Over!");
            onDeath();
        }
    }

    public void onDeath()
    {
        marioAnimator.Play("mario-death");
        marioAudio.PlayOneShot(marioDeath);
        alive = false;
    }

    public void setFaceRightState(bool state)
    {
        faceRightState = state;
    }

    #region INPUT CALLBACKS

    public void OnJumpInput()
    {
        LastPressedJumpTime = playerData.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
        {
            _isJumpCut = true;
        }
    }

    public void onDashInput()
    {
        LastPressedDashTime = playerData.dashInputBufferTime;
    }

    #endregion

    public void setGravityScale(float scale)
    {
        marioBody.gravityScale = scale;
    }

    private void setGroundState(bool state)
    {
        marioAnimator.SetBool("onGround", state);
        // Debug.Log("Setting ground state to: " + state);
    }

    private void Sleep(float duration)
    {
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    void PlayJumpSound()
    {
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
        GetComponent<BoxCollider2D>().enabled = false; // Disable collider to fall through everything
    }

    public void GameOverScene()
    {
        gameOverController.Setup(jumpOverGoomba.score);
    }

    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        float targetSpeed = _moveInput.x * playerData.runMaxSpeed;
        targetSpeed = Mathf.Lerp(marioBody.linearVelocity.x, targetSpeed, lerpAmount);

        float accelRate;

        if (LastOnGroundTime > 0)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount : playerData.runDeccelAmount;
        }
        else
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.runAccelAmount * playerData.accelInAir : playerData.runDeccelAmount * playerData.deccelInAir;
        }

        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(marioBody.linearVelocity.y) < playerData.jumpHangTimeThreshold)
        {
            accelRate *= playerData.jumpHangAccelerationMultiplier;
            targetSpeed *= playerData.jumpHangMaxSpeedMultiplier;
        }

        if (playerData.doConserveMomentum && Mathf.Abs(marioBody.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(marioBody.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            accelRate = 0;
        }

        float speedDiff = targetSpeed - marioBody.linearVelocity.x;
        float movement = speedDiff * accelRate;

        marioBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    private void Turn(bool shouldSkid = false)
    {

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;

        Debug.Log("Should skid: " + shouldSkid);
        if (shouldSkid)
        {
            marioAnimator.SetTrigger("onSkid");
            Debug.Log("Playing skid animation");
        }
    }

    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        Debug.Log("Jumping");
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        float force = playerData.jumpForce;
        if (marioBody.linearVelocityY < 0)
        {
            force -= marioBody.linearVelocityY;
        }

        marioBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        setGroundState(false);
    }

    private void WallJump(int dir)
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        // Always turn to face away from the wall first
        if ((dir == -1 && IsFacingRight) || (dir == 1 && !IsFacingRight))
        {
            Turn();
        }

        // Start the two-phase wall jump coroutine
        // StartCoroutine(TwoPhaseWallJump(dir));

        Vector2 force = new Vector2(playerData.wallJumpForce.x, playerData.wallJumpForce.y);
        force.x *= dir;

        if (Mathf.Sign(marioBody.linearVelocityX) != Mathf.Sign(force.x))
        {
            force.x -= marioBody.linearVelocityX;
        }

        if (marioBody.linearVelocityY < 0)
        {
            force.y -= marioBody.linearVelocityY;
        }

        marioBody.AddForce(force, ForceMode2D.Impulse);
    }

    private IEnumerator TwoPhaseWallJump(int dir)
    {
        float halfWallJumpTime = playerData.wallJumpTime / 2f;

        // Phase 1: Jump away from wall
        Vector2 awayForce = new Vector2(playerData.wallJumpForce.x * dir, playerData.wallJumpForce.y);

        // Handle existing velocity
        if (Mathf.Sign(marioBody.linearVelocityX) != Mathf.Sign(awayForce.x))
        {
            awayForce.x -= marioBody.linearVelocityX;
        }
        if (marioBody.linearVelocityY < 0)
        {
            awayForce.y -= marioBody.linearVelocityY;
        }

        marioBody.AddForce(awayForce, ForceMode2D.Impulse);
        Turn();
        Debug.Log("Phase 1: Jumping away from wall with force: " + awayForce);

        // Wait for first half of wall jump time
        yield return new WaitForSeconds(halfWallJumpTime);

        // Phase 2: Check if player wants to jump back towards wall
        bool movingTowardsWall = (_moveInput.x > 0 && dir == -1) || (_moveInput.x < 0 && dir == 1);

        if (movingTowardsWall)
        {
            Turn();
            Vector2 towardsForce = new Vector2(playerData.wallJumpForce.x * -dir, playerData.wallJumpForce.y);
            towardsForce.x -= marioBody.linearVelocityX;

            marioBody.AddForce(towardsForce, ForceMode2D.Impulse);
            Debug.Log($"Phase 2: Changed velocity to create V-shape. New velocity: {marioBody.linearVelocity}");
        }
        else
        {
            Debug.Log("Phase 2: No input towards wall, continuing normal trajectory");
        }
    }
    #endregion

    #region DASH METHODS

    private IEnumerator StartDash(Vector2 dir)
    {
        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTime = Time.time;

        _dashesLeft--;
        _isDashAttacking = true;

        setGravityScale(0);

        while (Time.time - startTime <= playerData.dashAttackTime)
        {
            marioBody.linearVelocity = dir.normalized * playerData.dashSpeed;
            yield return null;
        }

        startTime = Time.time;
        _isDashAttacking = false;

        setGravityScale(playerData.gravityScale);
        marioBody.linearVelocity = playerData.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= playerData.dashEndTime)
        {
            yield return null;
        }

        IsDashing = false;
    }

    private IEnumerator RefillDash(int amount)
    {
        _dashRefilling = true;
        yield return new WaitForSeconds(playerData.dashRefillTime);
        _dashRefilling = false;
        _dashesLeft = Mathf.Min(playerData.dashAmount, _dashesLeft + amount);
    }
    #endregion

    #region SLIDE METHODS
    private void Slide()
    {
        if (marioBody.linearVelocityY > 0)
        {
            Debug.Log("Sliding upwards, cancelling upward velocity");
            marioBody.AddForce(-marioBody.linearVelocityY * Vector2.up, ForceMode2D.Impulse);
        }

        float speedDiff = playerData.slideSpeed - marioBody.linearVelocityY;
        float movement = speedDiff * playerData.slideAccel;

        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime));

        marioBody.AddForce(movement * Vector2.up);
        Debug.Log("Sliding down wall with added force: " + (movement * Vector2.up));
    }

    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
        {
            bool shouldSkid = ShouldPlaySkidAnimation();
            Turn(shouldSkid);
        }
    }

    private bool ShouldPlaySkidAnimation()
    {
        // Check if player is moving fast enough in the opposite direction

        // If facing right but moving left with significant speed
        if (!IsJumping && !IsWallJumping && IsFacingRight && _moveInput.x < -0.1f)
        {
            return true;
        }
        // If facing left but moving right with significant speed
        else if (!IsJumping && !IsWallJumping && !IsFacingRight && _moveInput.x > 0.1f)
        {
            return true;
        }

        return false;
    }


    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        bool canWallJump = LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
        (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));

        // Debug.Log("CanWallJump: " + canWallJump);

        bool testWallJump = LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && ((LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));

        // Debug.Log("Test Wall Jump: " + testWallJump);

        // return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
        // (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
        return canWallJump;
    }

    private bool CanJumpCut()
    {
        return IsJumping && marioBody.linearVelocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && marioBody.linearVelocity.y > 0;
    }

    private bool CanDash()
    {
        if (!IsDashing && _dashesLeft < playerData.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return _dashesLeft > 0;
    }

    private bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
}