using SCPE;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeProfileProgressiveInterpolator : MonoBehaviour
{
    [SerializeField] private Volume _volume;
    [SerializeField] private VolumeProfile _maxVolumeProfile;

    private ColorAdjustments _maxColorAdjustments;
    private Sharpen _maxSharpen;
    private ChannelMixer _maxChannelMixer;
    private WhiteBalance _maxWhiteBalance;
    private Bloom _maxBloom;
    
    void Start()
    {
        _maxVolumeProfile.TryGet(typeof(ColorAdjustments), out _maxColorAdjustments);
        _maxVolumeProfile.TryGet(typeof(Sharpen), out _maxSharpen);
        _maxVolumeProfile.TryGet(typeof(ChannelMixer), out _maxChannelMixer);
        _maxVolumeProfile.TryGet(typeof(WhiteBalance), out _maxWhiteBalance);
        _maxVolumeProfile.TryGet(typeof(Bloom), out _maxBloom);
    }

    public void Progress(float val) 
    {
        ColorAdjustments colorAdjustments;
        Sharpen sharpen;
        ChannelMixer channelMixer;
        WhiteBalance whiteBalance;
        Bloom bloom;
        
        _volume.profile.TryGet(typeof(ColorAdjustments), out colorAdjustments);
        _volume.profile.TryGet(typeof(Sharpen), out sharpen);
        _volume.profile.TryGet(typeof(ChannelMixer), out channelMixer);
        _volume.profile.TryGet(typeof(WhiteBalance), out whiteBalance);
        _volume.profile.TryGet(typeof(Bloom), out bloom);

        colorAdjustments.postExposure.SetValue(new FloatParameter(val * _maxColorAdjustments.postExposure.value)); 
        colorAdjustments.contrast.SetValue(new FloatParameter(val * _maxColorAdjustments.contrast.value)); 
        colorAdjustments.hueShift.SetValue(new FloatParameter(val * _maxColorAdjustments.hueShift.value)); 
        colorAdjustments.saturation.SetValue(new FloatParameter(val * _maxColorAdjustments.saturation.value));
        colorAdjustments.SetDirty();
        
        sharpen.amount.SetValue(new FloatParameter(val * _maxSharpen.amount.value));
        sharpen.radius.SetValue(new FloatParameter(Utils.Map(val, 0f, 1f, 1f,_maxSharpen.radius.value)));
        sharpen.contrast.SetValue(new FloatParameter(Utils.Map(val, 0f, 1f, 1f,_maxSharpen.contrast.value)));
        sharpen.SetDirty();
        
        channelMixer.redOutRedIn.SetValue(new FloatParameter(Utils.Map(val,0,1,100,_maxChannelMixer.redOutRedIn.value)));
        channelMixer.redOutGreenIn.SetValue(new FloatParameter(Utils.Map(val,0,1,0,_maxChannelMixer.redOutGreenIn.value)));
        channelMixer.redOutBlueIn.SetValue(new FloatParameter(Utils.Map(val,0,1,0, _maxChannelMixer.redOutBlueIn.value)));
        
        channelMixer.greenOutRedIn.SetValue(new FloatParameter(Utils.Map(val,0,1,0,_maxChannelMixer.greenOutRedIn.value)));
        channelMixer.greenOutGreenIn.SetValue(new FloatParameter(Utils.Map(val,0,1,100,_maxChannelMixer.greenOutGreenIn.value)));
        channelMixer.greenOutBlueIn.SetValue(new FloatParameter(Utils.Map(val,0,1,0,_maxChannelMixer.greenOutBlueIn.value)));

        channelMixer.blueOutRedIn.SetValue(new FloatParameter(Utils.Map(val,0,1,0,_maxChannelMixer.blueOutRedIn.value)));
        channelMixer.blueOutGreenIn.SetValue(new FloatParameter(Utils.Map(val,0,1,0,_maxChannelMixer.blueOutGreenIn.value)));
        channelMixer.blueOutBlueIn.SetValue(new FloatParameter(Utils.Map(val,0,1,100,_maxChannelMixer.blueOutBlueIn.value)));
        channelMixer.SetDirty();    

        whiteBalance.temperature.SetValue(new FloatParameter(Utils.Map(val,0,1,0,_maxWhiteBalance.temperature.value)));
        whiteBalance.tint.SetValue(new FloatParameter(Utils.Map(val,0,1,0,_maxWhiteBalance.tint.value)));
        whiteBalance.SetDirty();
        
        bloom.threshold.SetValue(new FloatParameter(Utils.Map(val,0,1,1,_maxBloom.threshold.value)));
        bloom.SetDirty();
    }
}
