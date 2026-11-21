# wktime

A lightweight utility for parsing work-time records, calculating breaks and working hours, and displaying detailed time summaries.

## Background

wktime originated from a practical need that arose during a work experience at a company providing logistics services to online retailers that did not have warehouses of their own.

At the time, keeping track of working hours and breaks had become particularly important to me. Knowing that a clock-in or clock-out event had been registered by the company's system was not enough: I wanted to maintain my own independent record of these events and be able to verify the calculations myself.

The attendance system could record clock entries simply by scanning a chip, confirming each operation with the message "Gebucht". However, I preferred not to rely exclusively on the information stored by the company's device. I wanted an independent record that would not be affected by possible subsequent changes or processing of the data.

I therefore began writing down my working times throughout the day. I did this quickly, often on a piece of paper, using a compact notation. For example, `801` represented 08:01.

Once at home, I transferred the recorded times to a Microsoft Access database that I had created specifically for this purpose. The clock entries were stored as a simple sequence of numeric values separated by commas, for example:

`657,1252,1331,1644`

This sequence represented the events of the working day: arrival, the beginning and end of breaks, and departure.

The next challenge was reconstructing the actual time worked.

The logic was straightforward, but required some care: the first time represented the beginning of the working day, the last one its end, while the intermediate times had to be considered in pairs to identify break intervals. It was then necessary to calculate the duration of each break, add them together, and subtract the total from the time elapsed between the first and last clock entries.

Initially, I performed these calculations manually. Over time, however, it became clear that automating the entire process would be more practical.

That was when I started developing a program capable of directly interpreting the sequences of times I had already become accustomed to recording. I did not want to change my recording method; I simply wanted to eliminate the repetitive work required to turn those records into useful information.

After considering how to organize the process, I developed a dedicated class for handling clock entries. The program would need to recognize and sort the times, identify break intervals, and automatically calculate both the total duration of interruptions and the net working hours.

This practical need led to the creation of `WorkTimeData`, the component that forms the core of wktime's time-processing logic.

The project was subsequently adapted and turned into a command-line application, while retaining its original idea: provide a sequence of times and receive a clear reconstruction of the working day in return.

wktime did not originate as a programming exercise or as an attempt to develop a commercial product. It was created out of the need to maintain control over my own records, independently verify working times, and reduce repetitive manual work.

Ultimately, it is a small utility developed to address a very specific personal need: transforming a series of raw clock entries into a verifiable representation of the time spent at work.

## Features

- Parse work-time records from a compact text string.
- Automatically normalize time entries.
- Calculate total break duration.
- Calculate total working hours.
- Display individual break intervals.
- Generate normalized time data.
- Provide formatted summaries, including decimal hours.

## Requirements

- Windows 7 or later.
- .NET Framework 3.5.

## Building

Open `wktime.sln` with Visual Studio and build the solution.

The project targets .NET Framework 3.5.

## License

No license specified.
