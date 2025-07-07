using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;
    [SerializeField] private float imageWidthOffset = 10;

    private float _imageFullWidth;
    private float _imageHalfWidth;

    public void CalculateImageWidth()
    {
        _imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        _imageHalfWidth = _imageFullWidth / 2;
    }

    public void Move(float distanceToMove) => background.position += Vector3.right * (distanceToMove * parallaxMultiplier);

    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        float imageRightEdge = (background.position.x + _imageHalfWidth) - imageWidthOffset;
        float imageLeftEdge = (background.position.x - _imageHalfWidth) + imageWidthOffset;

        if (imageRightEdge < cameraLeftEdge)
            background.position += Vector3.right * _imageFullWidth;
        else if (imageLeftEdge > cameraRightEdge)
            background.position += Vector3.right * -_imageFullWidth;
    }
}