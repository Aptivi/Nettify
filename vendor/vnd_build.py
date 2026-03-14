import os
import subprocess
import sys


def vnd_prebuild():
    solution = os.path.dirname(os.path.abspath(__file__ + '/../'))

    # Get paths and create the output path
    autoconf_tools_convert = solution + "/assets/autoconfig/tools/convert.py"
    autoconf_ispdb_path = solution + "/assets/autoconfig/ispdb"
    output_ispdb_path = solution + "/assets/ispdb"
    if not os.path.isdir(output_ispdb_path):
        os.mkdir(output_ispdb_path)

    # Enumerate all XML files
    xmls = [d for d in os.scandir(autoconf_ispdb_path)
            if d.is_file() and '.xml' in d.name]
    for xml in xmls:
        # Generate ISP DB info for all XML files
        command = \
            f"\"{sys.executable}\" " \
            f"\"{autoconf_tools_convert}\" " \
            f"-a -d \"{output_ispdb_path}\" " \
            f"{xml.path}"
        result = subprocess.run(command, shell=True)
        if result.returncode != 0:
            raise Exception("Conversion failed: %i" % (result.returncode))

    # Enumerate all generated files
    genfiles = [d for d in os.scandir(output_ispdb_path)
                if d.is_file()]
    for genfile in genfiles:
        # Change the file extension
        genfile_xml = genfile.path + '.xml'
        os.rename(genfile, genfile_xml)
        print(f'Processed {genfile_xml}')


def vnd_build(args, extra_args):
    solution = os.path.dirname(os.path.abspath(__file__ + '/../'))
    solution = solution + "/Nettify.slnx"
    command = f"dotnet build {solution} {args if args else ''}"
    result = subprocess.run(command, shell=True)
    if result.returncode != 0:
        raise Exception("Build failed with code %i" % (result.returncode))
