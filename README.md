blux
====

b.lux is a very basic Windows program which changes the colour temperature of the display.

It is inspired by the fantasic [f.lux](https://justgetflux.com/), with a couple of differences:

First, unlike f.lux, the colour temperature changes are based on the *time of day* rather than the time of sunset. It may be dark outside at 4pm in winter, but I won't be going to bed until much later, so I don't want my display to turn orange in the mid-afternoon!

Secondly, the rate of colour temperature change is *very gradual*, taking place over a period of two hours. The theory is, the less perceptable the change, the greater likelihood of the user allowing it to take place (not closing the program or choosing "disable for an hour" etc).

At the moment, the times of day are hard-coded as follows:

* Upto 9pm: Default colour temperature of 6500K
* 9pm - 10pm: Colour temperature will gradually change to 3400K
* 10pm - 11pm: Colour temperature will gradually change to 1900K

Like I said, it's very basic.

The UI also allows for brightness adjustment and there are a number of presets, however these are unavailable if Timer mode is enabled (which is the default).

![Screenshot](blux/blux.png)
