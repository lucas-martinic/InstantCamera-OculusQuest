using System.Collections;
using UnityEngine;

public class Snapshot : MonoBehaviour
{
    Camera snapCam;
    bool takingPic;
    public GameObject photoPrefab;

    int resWidth = 256;
    int resHeight = 256;

    public Transform photoPos;

    float counter;
    bool printingPhoto;

    bool grabbedByL;
    bool grabbedByR;

    AudioSource audio;

    GameObject printingPic;

    Vector3 initialPos;
    Quaternion initialRot;

    public AudioClip dropClip;

    // Start is called before the first frame update
    void Awake()
    {
        snapCam = GetComponentInChildren<Camera>();
        if (snapCam.targetTexture == null)
        {
            snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24);
        }
        else
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
        audio = GetComponent<AudioSource>();

        initialPos = transform.position;
        initialRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            takingPic = true;
        }

        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) && grabbedByL && !printingPhoto)
        {
            takingPic = true;
        }
        else if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) && grabbedByR && !printingPhoto)
        {
            takingPic = true;
        }

        if (printingPhoto)
        {
            counter += Time.deltaTime;
            if (counter >= 2.5f)
            {
                printingPhoto = false;
                counter = 0;
            }
        }
    }

    public void TakeInstaShot()
    {
        takingPic = true;
    }

    void LateUpdate()
    {
        if (takingPic && !printingPhoto)
        {
            audio.Play();
            Texture2D snapshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            snapCam.Render();
            RenderTexture.active = snapCam.targetTexture;
            snapshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            snapshot.Apply();
            printingPic = Instantiate(photoPrefab, photoPos.position, Quaternion.identity);
            printingPic.transform.parent = transform;
            printingPic.transform.rotation = photoPos.rotation;
            GameObject picture = printingPic.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
            picture.GetComponent<Renderer>().material.mainTexture = snapshot;
            printingPhoto = true;
        }

        takingPic = false;
    }

    public void GrabbedByR()
    {
        grabbedByR = true;

    }

    public void GrabbedByL()
    {
        grabbedByL = true;
    } 

    public void UnGrabbed()
    {
        grabbedByR = false;
        grabbedByL = false;
    }

    public void ResetPos()
    {
        audio.PlayOneShot(dropClip, 0.7f);
        StartCoroutine(ResetCamPos());
    }

    IEnumerator ResetCamPos()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.position = initialPos;
        transform.rotation = initialRot;
    }
}
