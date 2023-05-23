import pandas as pd
from matplotlib import pyplot as plt
plt.rcParams["figure.figsize"] = [7.00, 3.50]
plt.rcParams["figure.autolayout"] = True
columns = ["A", "B", "C", "D"]
df = pd.read_csv("../src/InfoWave.MonoGame/bin/Debug/net7.0/data.csv", usecols=columns)
print("Contents in csv file:", df)
plt.plot(df.A, df.B, df.C)
plt.show()